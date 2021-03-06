﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using SupportSearch = Android.Support.V7.Widget.SearchView;

using SQLite;

namespace ConnectUTS
{
	public class FriendsFragment : Fragment
	{
		private List<Profile> mUsers;
		private SupportSearch mSearch;
		private ListView mUsersList;
		private FriendAdapter mAdapter;
		private Profile mCurrentUser;
		SQLiteConnection db;

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetHasOptionsMenu (true);
			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View view = inflater.Inflate(Resource.Layout.Friends, container, false);

			string path = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal);
			db = new SQLiteConnection (System.IO.Path.Combine(path, "Database.db"));

			mCurrentUser = db.Query<Profile>("SELECT * FROM Profile WHERE StudentId = '" + Arguments.GetString("studentID") + "'")[0];

			DisplayUsers (view);

			return view;
		}

		public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
		{
			inflater.Inflate (Resource.Menu.ActionMenu, menu);

			// Set up the search
			var search = menu.FindItem (Resource.Id.actionSearch);
			var searchView = MenuItemCompat.GetActionView (search);
			mSearch = searchView.JavaCast<SupportSearch> ();

			// Check for query and filter ListView
			mSearch.QueryTextChange += (sender, e) => mAdapter.Filter.InvokeFilter(e.NewText);
			mSearch.QueryTextSubmit += (sender, e) => {};
		}

		// Displays the all the users sorted by "match"
		private void DisplayUsers (View view)
		{
			mUsers = new List<Profile> ();
			foreach (Profile user in db.Query<Profile>("SELECT * FROM Profile"))
			{
				if (!(user.StudentID == mCurrentUser.StudentID))
				{
					mUsers.Add (user);
				}
			}

			mUsers = mUsers.OrderByDescending (user => user.GetRank (mCurrentUser)).ToList();

			// Add users to the mUsers list 
			// Inflate the listview with the mUsers list
			mUsersList = view.FindViewById<ListView>(Resource.Id.listFriends);
			mAdapter = new FriendAdapter (Activity, mUsers, mCurrentUser);
			mUsersList.Adapter = mAdapter;
		}
	}
}