using FairyGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatList : MonoBehaviour
{
	public struct ChatListItem
	{
		public string message;
		public bool fromMe;
		public ChatListItem(string message, bool fromMe)
		{
			this.message = message;
			this.fromMe = fromMe;
		}
	}

	private GList _list;
	private GComponent _component;
	private List<ChatListItem> _data;
	public FriendList _friendList;

	public void Add(string uid, string other, ChatListItem item)
	{
		_data.Add(item);
		_list.numItems++;
		DataService.AddToChatList(uid, other, item);
	}

	public ChatList()
	{
		_list = ChatListCom.GetChild("List").asList;
		_list.itemRenderer = RenderChatListItem;
		_list.itemProvider = ChatListItemProvider;
		_list.SetVirtual();
		ChatListCom.GetChild("Back").asButton.onClick.Add(onBackButtonClicked);
		ChatListCom.GetChild("Send").asButton.onClick.Add(onSendButtonClicked);
		ChatListData = new List<ChatListItem>();
	}

	private void GetChatListDataFromDB(string uid)
	{
		ChatListData.Clear();
		ChatListData = ds.GetChatList(UID, uid);
	}

	void onBackButtonClicked(EventContext context)
	{
		GRoot.inst.RemoveChildren();
		GRoot.inst.AddChild(FriendListCom);
		FriendList.RefreshVirtualList();
	}

	void onSendButtonClicked(EventContext context)
	{
		AddMessage(UID, FriendListData[Index].uid, ChatListCom.GetChild("Input").asTextInput.text, true);
		AddMessage(UID, FriendListData[Index].uid, "嗯嗯", false);
	}

	void RenderChatListItem(int Index, GObject obj)
	{
		obj.asCom.GetChild("message").asTextField.text = ChatListData[Index].message;
		if (!ChatListData[Index].fromMe)
		{
			obj.asCom.GetChild("name").asTextField.text = ChatListName;
		}
	}

	string ChatListItemProvider(int Index)
	{
		if (ChatListData[Index].fromMe)
		{
			return "ui://Package1/ChatRight";
		}
		else
		{
			return "ui://Package1/ChatLeft";
		}
	}
}
