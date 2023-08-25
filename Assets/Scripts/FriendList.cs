using FairyGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendList : MonoBehaviour
{
	public struct FriendListItem
	{
		public string title {  get; set; }
		public string message { get; set; }
		public string uid { get; set; }

		public FriendListItem(string title, string message, string uid)
		{
			this.title = title;
			this.message = message;
			this.uid = uid;
		}
	}

	private GList _list;
	private GComponent _component;
	private List<FriendListItem> _data;
	public Action<int, string> SwitchToChatList;
	public ChatList _chatlist;
	private string UID;
//	public string Name;
//	public int Index;

	public FriendList(string UID, Action<int, string> switcher)
	{
		this.UID = UID;
		this.SwitchToChatList = switcher;
		GetFriendListDataFromDB(UID);
		_component = UIPackage.CreateObject("Package1", "FriendList").asCom;
		_list = _component.GetChild("List").asList;
		_list.itemRenderer = RenderListItem;
		_list.onClickItem.Add(onFriendListItemClicked);
		_list.SetVirtual();
		_list.numItems = _data.Count;
	}

	public void Show()
	{
		_list.RefreshVirtualList();
		GRoot.inst.AddChild(_component);
	}

	public void Hide()
	{
		GRoot.inst.RemoveChild(_component);
	}

	public string GetUID(int index)
	{
		return _data[index].uid;
	}

	public void SetMessage(string uid, string message)
	{
		for (int i =  0; i < _data.Count; i++)
		{
			if (uid == _data[i].uid)
			{
				var tmp = _data[i];
				tmp.message = message;
				_data[i] = tmp;
			}
		}
		DataService.UpdateFriendListMessage(UID, uid, message);
	}

	void AddFriend(string other)
	{
		var item = new FriendListItem() { message = "", uid = other, title = other };
		_data.Add(item);
	}

	private void GetFriendListDataFromDB(string uid)
	{
		_data = DataService.GetFriendList(uid);
		if (_data.Count == 0)
		{
			for (int i = 0; i < 5; i++)
			{
				_data.Add(new FriendListItem("friend" + i, "message" + i, "uid" + i));
				DataService.AddToFriendList(uid, "uid" + i);
			}
		}
	}

	void RenderListItem(int Index, GObject obj)
	{
		GButton btn = obj.asButton;
		btn.title = _data[Index].title;
		btn.GetChild("message").asTextField.text = _data[Index].message;
	}

	void onFriendListItemClicked(EventContext context)
	{
		int Index = _list.GetChildIndex((GObject)context.data);
		string Name = _data[Index].title;
		SwitchToChatList(Index, Name);
	}
}
