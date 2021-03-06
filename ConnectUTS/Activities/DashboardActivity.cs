﻿

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Support.V4.Widget;

namespace ConnectUTS
{
	[Activity (Label = "Dashboard", Theme = "@style/ConnectUtsTheme")]				
	public class DashboardActivity : AppCompatActivity
	{
		private SupportToolbar mToolbar;
		private int mCurrentViewTitle = Resource.String.app_name;
		private DashboardToggle mDashboardToggle;
		private DrawerLayout mDrawerLayout;
		private ArrayAdapter mDashboardAdapter;
		private ListView mDashboard;
		private FragmentTransaction mFragmentManager;
		private Fragment mFriends;
		private Fragment mProfile;
		private Fragment mBed;
		private Fragment mHousemate;
		private Bundle userIDHolder;
		private string studentID = String.Empty;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Dashboard);

			studentID = Intent.Extras.GetString ("studentID");

			mDrawerLayout = FindViewById<DrawerLayout> (Resource.Id.dashboardDrawer);
			mToolbar = FindViewById<SupportToolbar> (Resource.Id.toolbar);

			// Set up the fragments
			mFriends = new FriendsFragment();
			mProfile = new ProfilePageFragment ();
			mBed = new AccommodationFragment ();
			mHousemate = new FindHousemateFragment ();

			// Set up userIdHolder
			userIDHolder = new Bundle();
			userIDHolder.PutString("studentID", studentID);
			mProfile.Arguments = userIDHolder;
			mFriends.Arguments = userIDHolder;
			mBed.Arguments = userIDHolder;
			mHousemate.Arguments = userIDHolder;

			// Sets up the toggle for the dashboard drawer.
			mDashboardToggle = new DashboardToggle (this, mDrawerLayout, Resource.String.menu_title, mCurrentViewTitle);
			mDrawerLayout.SetDrawerListener (mDashboardToggle);

			// Displays the custom action bar.
			DisplayToolbar (bundle);

			mDashboardToggle.SyncState();

			// Controls the dashboard.
			InflateDashboard();
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			mDashboardToggle.OnOptionsItemSelected(item);
			return base.OnOptionsItemSelected(item);
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			if (mDrawerLayout.IsDrawerOpen((int)GravityFlags.Left))
			{
				outState.PutString("DrawerState", "Opened");
			}

			else
			{
				outState.PutString("DrawerState", "Closed");
			}

			base.OnSaveInstanceState(outState);
		}

		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
		}

		private void DisplayToolbar(Bundle bundle)
		{
			SetSupportActionBar(mToolbar);
			SupportActionBar.SetHomeButtonEnabled(true);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);

			// Not the first time activity has been run.
			if (bundle != null)
			{
				if (bundle.GetString("DrawerState") == "Opened")
				{
					SupportActionBar.SetTitle(Resource.String.menu_title);
				}
				else
				{
					SupportActionBar.SetTitle(mCurrentViewTitle);
				}
			}
			// First time activity has been run.
			else
			{
				SupportActionBar.SetTitle(mCurrentViewTitle);
			}
		}

		private void InflateDashboard()
		{
			mDashboard = FindViewById<ListView> (Resource.Id.menuList);
			mDashboardAdapter = ArrayAdapter<string>.CreateFromResource (this, Resource.Array.menu, Resource.Layout.DashboardRowLayout);

			mDashboard.Adapter = mDashboardAdapter;

			mDashboard.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => {
				switch (e.Position) {
				case 0:
					// Profile page
					mCurrentViewTitle = Resource.String.app_name;
					SetView(Resource.Id.fragmentContainer, mProfile, true);
					break;
				case 1:
					// Find friends - suggested users
					mCurrentViewTitle = Resource.String.friends_title;
					SetView(Resource.Id.fragmentContainer, mFriends, true);
					break;
				case 2:
					// Find accommodation
					mCurrentViewTitle = Resource.String.bed_title;
					SetView(Resource.Id.fragmentContainer, mBed, true);
					break;
				case 3:
					// Find a housemate
					mCurrentViewTitle = Resource.String.housemate_title;
					SetView(Resource.Id.fragmentContainer, mHousemate, true);
					break;
				case 4:
					// Settings
					mCurrentViewTitle = Resource.String.settings_title;
					break;
				case 5:
					// Log Out
					var intent = new Intent(this, typeof(MainActivity));
					StartActivity(intent);
					// Stops user from pressing back button to return.
					Finish();
					break;
				}
			};
		}

		// Sets the toolbar title, switches the views and closes the dashboard.
		private void SetView(int fragmentResource, Fragment view, bool retainView)
		{
			mDashboardToggle.SetClosedTitle(mCurrentViewTitle);

			mFragmentManager = FragmentManager.BeginTransaction ();
			mFragmentManager.Replace (fragmentResource, view);

			// If true, allows the user to return to that fragment.
			// Otherwise it is destroyed.
			if(retainView)
			{
				mFragmentManager.AddToBackStack(null);
			}

			mFragmentManager.Commit ();

			mDrawerLayout.CloseDrawers ();
		}
	}
}

