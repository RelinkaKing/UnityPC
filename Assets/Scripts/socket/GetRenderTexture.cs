using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GetRenderTexture : MonoBehaviour {
 	Texture2D tempTex;
 	Texture2D remoteTexture;
	public void SenPNG () {
		
		// if (Network.peerType == NetworkPeerType.Server || Network.peerType == NetworkPeerType.Client)
		// {
		// 	byte[] bytesToSend = tempTex.EncodeToPNG();
		// 	networkView.RPC("ReceivePNG", RPCMode.Others, bytesToSend);
		// }

		// byte[] colourArray = SerializeObject<Color[]>(GetRenderTexturePixels(myRenderTexture));
	}
	



	public void ReceivePNG(byte[] bytes)
	{
		if (bytes.Length < 1)
		{
			Debug.LogError("Received bad byte count from network.");
			return;
		}
		
		remoteTexture.LoadImage(bytes);

		//now apply your new texture as you please
	}

	public Color[] GetRenderTexturePixels(RenderTexture tex)
	{
		RenderTexture.active = tex;
		Texture2D tempTex = new Texture2D(tex.width, tex.height);
		tempTex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
		tempTex.Apply();
		return tempTex.GetPixels();
	}

	byte[] SerializeObject<T>(T objectToSerialize)
	{
		BinaryFormatter bf = new BinaryFormatter();
		MemoryStream memStr = new MemoryStream();
		bf.Serialize(memStr, objectToSerialize);
		memStr.Position = 0;
		//return "";
		return memStr.ToArray();
	}


	T DeserializeObject<T>(byte[] dataStream)
	{
		MemoryStream stream = new MemoryStream(dataStream);
		stream.Position = 0;
		BinaryFormatter bf = new BinaryFormatter();
		bf.Binder = new VersionFixer();
		T retV = (T)bf.Deserialize(stream);
		return retV;
	}


	sealed class VersionFixer : SerializationBinder 
	{
		public override Type BindToType(string assemblyName, string typeName) 
		{
			Type typeToDeserialize = null;
			// For each assemblyName/typeName that you want to deserialize to
			// a different type, set typeToDeserialize to the desired type.
			string assemVer1 = Assembly.GetExecutingAssembly().FullName;
			if (assemblyName != assemVer1) 
			{
				// To use a type from a different assembly version, 
				// change the version number.
				// To do this, uncomment the following line of code.
				assemblyName = assemVer1;
				// To use a different type from the same assembly, 
				// change the type name.
			}
			// The following line of code returns the type.
			typeToDeserialize = Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));
			return typeToDeserialize;
		}
	}
}
