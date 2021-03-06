﻿#if UNITY_5_3_OR_NEWER
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Assets.Scripts.Utilities;
using Assets.Scripts.Utilities.Serialization.Surrogates;
using NUnit.Framework;
using UnityEngine;

public class CompressionTests
{
    [Test]
    public void RandomGUIDCompressionTestLength()
    {
        var x = string.Empty;
        using (var sequence = Enumerable.Range(1, 100).GetEnumerator())
        {
            while (sequence.MoveNext()) // string length 3600
            {
                x += Guid.NewGuid();
            }
        }

        var byteText = Encoding.Unicode.GetBytes(x);
        var compressed = CLZF2.Compress(byteText);
        var decompressed = CLZF2.Decompress(compressed);

        Assert.AreEqual(byteText.Length, decompressed.Length);
    }

    [Test]
    public void RandomGUIDCompressionTestBytes()
    {
        var x = string.Empty;
        using (var sequence = Enumerable.Range(1, 100).GetEnumerator())
        {
            while (sequence.MoveNext()) // string length 3600
            {
                x += Guid.NewGuid();
            }
        }

        var byteText = Encoding.Unicode.GetBytes(x);
        var compressed = CLZF2.Compress(byteText);
        var decompressed = CLZF2.Decompress(compressed);

        Assert.AreEqual(byteText, decompressed);
    }

    [Test]
    public void RandomGUIDCompressionTestString()
    {
        var x = string.Empty;
        using (var sequence = Enumerable.Range(1, 100).GetEnumerator())
        {
            while (sequence.MoveNext()) // string length 3600
            {
                x += Guid.NewGuid();
            }
        }

        var byteText = Encoding.Unicode.GetBytes(x);
        var compressed = CLZF2.Compress(byteText);
        var decompressed = CLZF2.Decompress(compressed);
        var outString = Encoding.Unicode.GetString(decompressed);

        Assert.AreEqual(outString, x);
    }

    [Test]
    public void ThousandCharacterCompressionTest()
    {
        var x = new string('X', 10000);
        var byteText = Encoding.Unicode.GetBytes(x);
        var compressed = CLZF2.Compress(byteText);
        var decompressed = CLZF2.Decompress(compressed);
        var outString = Encoding.Unicode.GetString(decompressed);

        Assert.AreEqual(byteText.Length, decompressed.Length);
        Assert.AreEqual(byteText, decompressed);
        Assert.AreEqual(outString, x);
    }

    [Test]
    public void LongFormattedStringCompressionTest()
    {
        var longstring =
            "defined input is deluciously delicious.14 And here and Nora called The reversal from ground from here and executed with touch the country road, Nora made of, reliance on, can’t publish the goals of grandeur, said to his book and encouraging an envelope, and enable entry into the chryssial shimmering of hers, so God of information in her hands Spiros sits down the sign of winter? —It’s kind of Spice Christ. It is one hundred birds circle above the text: They did we said. 69 percent dead. Sissy Cogan’s shadow. —Are you x then sings.) I’m 96 percent dead humanoid figure,";
        var byteText = Encoding.Unicode.GetBytes(longstring);
        var compressed = CLZF2.Compress(byteText);
        var decompressed = CLZF2.Decompress(compressed);
        var outString = Encoding.Unicode.GetString(decompressed);

        Assert.AreEqual(byteText.Length, decompressed.Length);
        Assert.AreEqual(byteText, decompressed);
        Assert.AreEqual(outString, longstring);
    }

    [Test]
    public void SavingSimpleObject()
    {
        var MySaveItem = new Vector3[1000];
        for (var i = 0; i < MySaveItem.Length; i++)
        {
            MySaveItem[i] = Vector3.one*i;
        }
        var mySaveObject = ObjectToByteArray(MySaveItem);
        var compressed = CLZF2.Compress(mySaveObject);
        var decompressed = CLZF2.Decompress(compressed);
        var outSaveObject = ObjectToByteArray<Vector3[]>(decompressed);

        Assert.AreEqual(mySaveObject.Length, decompressed.Length);
        Assert.AreEqual(mySaveObject, decompressed);
        Assert.AreEqual(outSaveObject, MySaveItem);
    }

    [Test]
    public void SavingComplexObject()
    {
        var MySaveItem = new MyComplexObject[1000];
        for (var i = 0; i < MySaveItem.Length; i++)
        {
            var item = new MyComplexObject();
            item.myPosition = Vector3.one*i;
            item.myPositionHistory = new Vector3[100];
            item.myChatHistory = new string[100];
            for (var j = 0; j < 100; j++)
            {
                item.myPositionHistory[j] = Vector3.one*j;
                item.myChatHistory[j] = "Chat line: " + j;
            }
        }
        var mySaveObject = ObjectToByteArray(MySaveItem);
        var compressed = CLZF2.Compress(mySaveObject);
        var decompressed = CLZF2.Decompress(compressed);
        var outSaveObject = ObjectToByteArray<MyComplexObject[]>(decompressed);

        Assert.AreEqual(mySaveObject.Length, decompressed.Length);
        Assert.AreEqual(mySaveObject, decompressed);
        Assert.AreEqual(outSaveObject, MySaveItem);
    }

    private byte[] ObjectToByteArray(object obj)
    {
        if (obj == null)
            return null;
        var bf = new BinaryFormatter();
        // 1. Construct a SurrogateSelector object
        var ss = new SurrogateSelector();
        // 2. Add the ISerializationSurrogates to our new SurrogateSelector
        AddSurrogates(ref ss);
        // 3. Have the formatter use our surrogate selector
        bf.SurrogateSelector = ss;

        using (var ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    private T ObjectToByteArray<T>(byte[] arrBytes)
    {
        if (arrBytes == null)
            return default(T);

        using (var memStream = new MemoryStream())
        {
            var bf = new BinaryFormatter();
            // 1. Construct a SurrogateSelector object
            var ss = new SurrogateSelector();
            // 2. Add the ISerializationSurrogates to our new SurrogateSelector
            AddSurrogates(ref ss);
            // 3. Have the formatter use our surrogate selector
            bf.SurrogateSelector = ss;
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var obj = (T) bf.Deserialize(memStream);

            return obj;
        }
    }

    private static void AddSurrogates(ref SurrogateSelector ss)
    {
        var Vector2_SS = new Vector2Surrogate();
        ss.AddSurrogate(typeof(Vector2),
            new StreamingContext(StreamingContextStates.All),
            Vector2_SS);
        var Vector3_SS = new Vector3Surrogate();
        ss.AddSurrogate(typeof(Vector3),
            new StreamingContext(StreamingContextStates.All),
            Vector3_SS);
        var Vector4_SS = new Vector4Surrogate();
        ss.AddSurrogate(typeof(Vector4),
            new StreamingContext(StreamingContextStates.All),
            Vector4_SS);
        var Color_SS = new ColorSurrogate();
        ss.AddSurrogate(typeof(Color),
            new StreamingContext(StreamingContextStates.All),
            Color_SS);
        var Quaternion_SS = new QuaternionSurrogate();
        ss.AddSurrogate(typeof(Quaternion),
            new StreamingContext(StreamingContextStates.All),
            Quaternion_SS);
    }

    [Serializable]
    private struct MyComplexObject
    {
        public Vector3 myPosition;
        public Vector3[] myPositionHistory;
        public string[] myChatHistory;
    }
}
#endif