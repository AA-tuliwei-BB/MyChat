using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
	static ChatManager instance = new ChatManager();
	public static ChatManager inst
	{
		get
		{
			return instance;
		}
	}

	private FriendList friendList;
	private ChatList chatList;

	ChatManager()
	{
	}

	void AddMessage(string uid, string other, string message, bool fromMe)
	{
		var item = new ChatList.ChatListItem(message, fromMe);
		chatList.Add(uid, other, item);
		friendList.SetMessage(other, message);
	}
}
