using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System;


public class XmlSaver
{   
    public static string EncryptObject(object pObject, System.Type ty){
      return Encrypt(SerializeObject(pObject,ty));
    }

    public static object DecryptObject(string toD, System.Type ty){
      return DeserializeObject(Decrypt(toD),ty );
    }

    public static string Encrypt(string toE)
    {
		byte[] keyArray = UTF8Encoding.UTF8.GetBytes("00000000000000000000000000000000");
        RijndaelManaged rDel = new RijndaelManaged();
        rDel.Key = keyArray;
        rDel.Mode = CipherMode.ECB;
        rDel.Padding = PaddingMode.PKCS7;
        ICryptoTransform cTransform = rDel.CreateEncryptor();
       
        byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toE);
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray,0,toEncryptArray.Length);
   
        return Convert.ToBase64String(resultArray,0,resultArray.Length);
    }
   

    public static string Decrypt(string toD)
    {
		byte[] keyArray = UTF8Encoding.UTF8.GetBytes("00000000000000000000000000000000");
       
        RijndaelManaged rDel = new RijndaelManaged();
        rDel.Key = keyArray;
        rDel.Mode = CipherMode.ECB;
        rDel.Padding = PaddingMode.PKCS7;
        ICryptoTransform cTransform = rDel.CreateDecryptor();
       
        byte[] toEncryptArray = Convert.FromBase64String(toD);
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray,0,toEncryptArray.Length);
       
        return UTF8Encoding.UTF8.GetString(resultArray);
    }
   
    public static string SerializeObject(object pObject,System.Type ty)
    {
       string XmlizedString   = null;
       MemoryStream memoryStream  = new MemoryStream();
       XmlSerializer xs  = new XmlSerializer(ty);
       XmlTextWriter xmlTextWriter  = new XmlTextWriter(memoryStream, Encoding.UTF8);
       xs.Serialize(xmlTextWriter, pObject);
       memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
       XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
       return XmlizedString;
    }
   
    public static object DeserializeObject(string pXmlizedString , System.Type ty)
    {
       XmlSerializer xs  = new XmlSerializer(ty);
       MemoryStream memoryStream  = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
       XmlTextWriter xmlTextWriter   = new XmlTextWriter(memoryStream, Encoding.UTF8);
       return xs.Deserialize(memoryStream);
    }
 
	public static void CreateFile(string fileName,string thisData)
	{
		//string xxx = Encrypt(thisData);
		StreamWriter writer;
		writer = File.CreateText(fileName);
		writer.Write(thisData);
		writer.Close();
	}

	public static string LoadFile(string fileName)
	{
		StreamReader sReader = File.OpenText(fileName);
		string dataString = sReader.ReadToEnd();
		sReader.Close();
		return dataString;
	}


    public static void CreateXML(string fileName,string thisData)
    {
       string xxx = Encrypt(thisData);
       StreamWriter writer;
       writer = File.CreateText(fileName);
       writer.Write(xxx);
       writer.Close();
    }
   
 
    public static string LoadXML(string fileName)
    {
       StreamReader sReader = File.OpenText(fileName);
       string dataString = sReader.ReadToEnd();
       sReader.Close();
       string xxx = Decrypt(dataString);
       return xxx;
   	}

    public static bool hasFile(String fileName)
    {
       return File.Exists(fileName);
    }

    public static string UTF8ByteArrayToString(byte[] characters  )
    {    
       UTF8Encoding encoding  = new UTF8Encoding();
       string constructedString  = encoding.GetString(characters);
       return (constructedString);
    }
   
    public static byte[] StringToUTF8ByteArray(String pXmlString )
    {
       UTF8Encoding encoding  = new UTF8Encoding();
       byte[] byteArray  = encoding.GetBytes(pXmlString);
       return byteArray;
    }



}