using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System;

public class GameSaveLoadEncrypted : MonoBehaviour
{
    // declare here what you're gonna to save or load
    bool _ShouldSave, _ShouldLoad, _SwitchSave, _SwitchLoad;
    string _FileLocation, _FileName;
    public GameObject _Player;
    UserData myData;
    string _data;
	static string data;

	// set here your filename
    void Awake()
    {
        _FileLocation = Application.dataPath;
        _FileName = "FILENAME.xml";
        myData = new UserData();
        Load_();

    }

    //unused
    void Start()
    {
        
    }

    //on the left you have your saving instance and you have to assign him all the values you want to store
    public void Save_M()
    {
        myData._iUser.MusicVolumeAudio = gameObject.GetComponent<Audio_>().Volum;
        myData._iUser.MB = gameObject.GetComponent<Menu_Navigation>().MB_.isOn;
        myData._iUser.AO = gameObject.GetComponent<Menu_Navigation>().AO_.isOn;
        myData._iUser.AA = gameObject.GetComponent<Menu_Navigation>().AA_.isOn;
        myData._iUser.QualityLevel_ = gameObject.GetComponent<Menu_Navigation>().ResInt;
        myData._iUser.Windoweed = gameObject.GetComponent<Menu_Navigation>().ONOFF;
        _data = SerializeObject(myData);
        _data = Encrypt(_data);
        CreateXML();
    }

    // on the left you have all the values you want to update and on the right the data read from the XML file.
    public void Load_()
    {
        LoadXML();
        _data = Decrypt(_data);
        if (_data.ToString() != "")
        myData = (UserData)DeserializeObject(_data);
        gameObject.GetComponent<Audio_>().Volum = myData._iUser.MusicVolumeAudio;
        gameObject.GetComponent<Menu_Navigation>().MB_.isOn = myData._iUser.MB ;
        gameObject.GetComponent<Menu_Navigation>().AO_.isOn = myData._iUser.AO ;
        gameObject.GetComponent<Menu_Navigation>().AA_.isOn = myData._iUser.AA  ;
        gameObject.GetComponent<Menu_Navigation>().ResInt = myData._iUser.QualityLevel_ ;
        gameObject.GetComponent<Menu_Navigation>().ONOFF = myData._iUser.Windoweed;

    }

    //encoding characters in UTF8
    string UTF8ByteArrayToString(byte[] characters)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        string constructedString = encoding.GetString(characters);
		print (data);
        return (constructedString);
    }

    //byte array
    byte[] StringToUTF8ByteArray(string pXmlString)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        byte[] byteArray = encoding.GetBytes(pXmlString);
        return byteArray;
    }

    // serializing the data
    string SerializeObject(object pObject)
    {
        string XmlizedString = null;
        MemoryStream memoryStream = new MemoryStream();
        XmlSerializer xs = new XmlSerializer(typeof(UserData));
        XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
        xs.Serialize(xmlTextWriter, pObject);
        memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
        XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
        return XmlizedString;
    }

    // deserializing the data
    object DeserializeObject(string pXmlizedString)
    {
        XmlSerializer xs = new XmlSerializer(typeof(UserData));
        MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
        XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
        return xs.Deserialize(memoryStream);
    }

    // now we're gonna create the XML file
    void CreateXML()
    {
        StreamWriter writer;
        FileInfo t = new FileInfo(_FileLocation + "\\" + _FileName);
        if (!t.Exists)
        {
            writer = t.CreateText();
        }
        else
        {
            t.Delete();
            writer = t.CreateText();
        }
        writer.Write(_data);
        writer.Close();
  
    }

    // We read the XML file
    void LoadXML()
    {
        StreamReader r = File.OpenText(_FileLocation + "\\" + _FileName);
        string _info = r.ReadToEnd();
        r.Close();
        _data = _info;
	}

	//function to encrypt your data using AES-256
	public static string Encrypt (string toEncrypt)
	{
		byte[] keyArray = UTF8Encoding.UTF8.GetBytes ("INSERTHEREYOURKEY");
		// 256-AES key
		byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes (toEncrypt);
		RijndaelManaged rDel = new RijndaelManaged ();
		rDel.Key = keyArray;
		rDel.Mode = CipherMode.ECB;
		// http://msdn.microsoft.com/en-us/library/system.security.cryptography.ciphermode.aspx
		rDel.Padding = PaddingMode.PKCS7;
		// better lang support
		ICryptoTransform cTransform = rDel.CreateEncryptor ();
		byte[] resultArray = cTransform.TransformFinalBlock (toEncryptArray, 0, toEncryptArray.Length);
		return Convert.ToBase64String (resultArray, 0, resultArray.Length);
	}

    // Function able to decrypt your AES-256 encrypted file
	public static string Decrypt (string toDecrypt)
	{
		byte[] keyArray = UTF8Encoding.UTF8.GetBytes ("INSERTHEREYOURKEY");
		// AES-256 key
		byte[] toEncryptArray = Convert.FromBase64String (toDecrypt);
		RijndaelManaged rDel = new RijndaelManaged ();
		rDel.Key = keyArray;
		rDel.Mode = CipherMode.ECB;
		// http://msdn.microsoft.com/en-us/library/system.security.cryptography.ciphermode.aspx
		rDel.Padding = PaddingMode.PKCS7;
		// better lang support
		ICryptoTransform cTransform = rDel.CreateDecryptor ();
		byte[] resultArray = cTransform.TransformFinalBlock (toEncryptArray, 0, toEncryptArray.Length);
		return UTF8Encoding.UTF8.GetString (resultArray);
	}
}

// this is the class that is going to store the variables to be written and to be read.
public class UserData
{
    //default instance
    public DemoData _iUser;
    // default constructor for the class
    public UserData() { }

    // here we have to define the variables we're going to save. here you have an example of data...
    public struct DemoData
    {
        public float MusicVolumeAudio;
        public float VoiceVolumeAudio;
        public float SfxVolumeAUdio;
        public bool MB;
        public bool AO;
        public bool AA;
        public int QualityLevel_;
        public bool Windoweed;
    }
}