using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ViewManager : MonoBehaviour
{
	static ViewManager instance = new ViewManager();
	public static ViewManager inst
	{
		get
		{
			return instance;
		}
	}

	ViewManager()
	{
		views = new List<View>();
	}

	public interface View {
		void Show();
		void Hide();
	}

	List<View> views;

	public void Back()
	{
		if (views.Count <= 1)
		{
			return;
		}
		View now = views[views.Count - 1];
		View back = views[views.Count - 2];
		now.Hide();
		back.Show();
	}

	public void Open(View view)
	{
		View now = views[views.Count - 1];
		views.Add(view);
		now.Hide();
		view.Show();
	}
}
