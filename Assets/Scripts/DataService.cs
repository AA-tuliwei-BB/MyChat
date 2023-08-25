using SQLite4Unity3d;
using UnityEngine;
using System.Linq;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;

public static class DataService  {

	private static SQLiteConnection _connection;

	public static void Init(string DatabaseName){

#if UNITY_EDITOR
            var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID 
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
		var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
		
#elif UNITY_STANDALONE_OSX
		var loadDb = Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        Debug.Log("Final PATH: " + dbPath);
	}

	public static void CreateDB(){
		_connection.CreateTable<FriendListDBItem> ();
		_connection.CreateTable<ChatListDBItem> ();
	}

	public static List<FriendList.FriendListItem> GetFriendList(string uid)
	{
		List<FriendList.FriendListItem> ret = new List<FriendList.FriendListItem>();
		var table = _connection.Table<FriendListDBItem>().Where (x => x.uid == uid);
		foreach (var item in table) { ret.Add(item.ToFriendListItem()); }
		return ret;
	}

	public static List<ChatList.ChatListItem> GetChatList(string uid, string other)
	{
		List<ChatList.ChatListItem> ret = new List<ChatList.ChatListItem>();
		var table = _connection.Table<ChatListDBItem>().Where (x => x.uid == uid && x.other == other);
		foreach (var item in table) { ret.Add(item.ToChatListItem()); }
		return ret;
	}

	public static void AddToChatList(string uid, string other, ChatList.ChatListItem chat)
	{
		var Item = new ChatListDBItem { uid = uid, other = other, message = chat.message, fromMe = chat.fromMe ? 1 : 0 };
		_connection.Insert(Item);
	}

	public static void AddToFriendList(string uid, string other)
	{
		var Item = new FriendListDBItem { uid = uid, frienduid = other, message = "", title = other };
		_connection.Insert(Item);
	}

	public static void UpdateFriendListMessage(string uid, string other, string message)
	{
		var obj = _connection.Table<FriendListDBItem> ().Where(x => x.uid == uid && x.frienduid == other).FirstOrDefault();
		obj.message = message;
		_connection.Update(obj);
	}

	/*
	public IEnumerable<Person> GetPersons(){
		return _connection.Table<Person>();
	}

	public IEnumerable<Person> GetPersonsNamedRoberto(){
		return _connection.Table<Person>().Where(x => x.Name == "Roberto");
	}

	public Person GetJohnny(){
		return _connection.Table<Person>().Where(x => x.Name == "Johnny").FirstOrDefault();
	}

	public Person CreatePerson(){
		var p = new Person{
				Name = "Johnny",
				Surname = "Mnemonic",
				Age = 21
		};
		_connection.Insert (p);
		return p;
	}
	*/
}

public class FriendListDBItem
{
	[PrimaryKey, AutoIncrement]
	public int Id { get; set; }

	public string uid { get; set; }
	//public MyChat.FriendListItem friend;
	public string title { get; set; }
	public string message { get; set; }
	public string frienduid { get; set; }

	public FriendList.FriendListItem ToFriendListItem()
	{
		return new FriendList.FriendListItem() { title = title, message = message, uid = frienduid };
	}
}

public class ChatListDBItem
{
	[PrimaryKey, AutoIncrement]
	public int Id { get; set; }

	public string uid { get; set; }
	public string other { get; set; }
	public string message { get; set; }
	public int fromMe { get; set; }
	public ChatList.ChatListItem ToChatListItem()
	{
		return new ChatList.ChatListItem() { message = message, fromMe = fromMe == 1 };
	}
}
