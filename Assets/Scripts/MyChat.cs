using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System.IO;
using System;

public class MyChat : MonoBehaviour
{


	private FriendList fl;
	private ChatList cl;

	private GComponent ChatListCom;
	private GList ChatList;
	[SerializeField]
	private List<ChatListItem> ChatListData;
	private string ChatListName;
	private int Index;
	private string UID;

	private void ChatListInit()
	{
		ChatList = ChatListCom.GetChild("List").asList;
		ChatList.itemRenderer = RenderChatListItem;
		ChatList.itemProvider = ChatListItemProvider;
		ChatList.SetVirtual();
		ChatListCom.GetChild("Back").asButton.onClick.Add(onBackButtonClicked);
		ChatListCom.GetChild("Send").asButton.onClick.Add(onSendButtonClicked);
		ChatListData = new List<ChatListItem>();
	}

	// Start is called before the first frame update
	void Start()
	{
		Application.targetFrameRate = 60;
		GRoot.inst.SetContentScaleFactor(720, 1280, UIContentScaler.ScreenMatchMode.MatchWidthOrHeight);

		DataService.Init("mychat.db");
		DataService.CreateDB();
		UID = "MyUID";
		fl = new FriendList(UID, FriendToChat);

		UIPackage.AddPackage("Package1");
		ChatListCom = UIPackage.CreateObject("Package1", "ChatList").asCom;
		fl.Show();

		ChatListInit();
//		Debug.Log(Application.persistentDataPath);
	}

	void FriendToChat(int index, string name)
	{
		GetChatListDataFromDB(fl.GetUID(index));
		ChatListCom.GetChild("title").asTextField.text = ChatListName;
		ChatList.numItems = ChatListData.Count;
		GRoot.inst.RemoveChildren();
		GRoot.inst.AddChild(ChatListCom);
	}

	private void GetChatListDataFromDB(string uid)
	{
		ChatListData.Clear();
		ChatListData = DataService.GetChatList(UID, uid);
	}

	private void SwitchToChatList()
	{
		GetChatListDataFromDB(FriendListData[Index].uid);
		ChatListCom.GetChild("title").asTextField.text = ChatListName;
		ChatList.numItems = ChatListData.Count;
		GRoot.inst.RemoveChildren();
		GRoot.inst.AddChild(ChatListCom);
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

	void RenderListItem(int Index, GObject obj)
	{
		GButton btn = obj.asButton;
		btn.title = FriendListData[Index].title;
		btn.GetChild("message").asTextField.text = FriendListData[Index].message;
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
