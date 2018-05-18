package mono.android.support.v4.view;


public class MenuItemCompat_OnActionExpandListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.support.v4.view.MenuItemCompat.OnActionExpandListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onMenuItemActionCollapse:(Landroid/view/MenuItem;)Z:GetOnMenuItemActionCollapse_Landroid_view_MenuItem_Handler:Android.Support.V4.View.MenuItemCompat/IOnActionExpandListenerInvoker, Xamarin.Android.Support.Compat\n" +
			"n_onMenuItemActionExpand:(Landroid/view/MenuItem;)Z:GetOnMenuItemActionExpand_Landroid_view_MenuItem_Handler:Android.Support.V4.View.MenuItemCompat/IOnActionExpandListenerInvoker, Xamarin.Android.Support.Compat\n" +
			"";
<<<<<<< Updated upstream
		mono.android.Runtime.register ("Android.Support.V4.View.MenuItemCompat+IOnActionExpandListenerImplementor, Xamarin.Android.Support.Compat, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", MenuItemCompat_OnActionExpandListenerImplementor.class, __md_methods);
=======
		mono.android.Runtime.register ("Android.Support.V4.View.MenuItemCompat+IOnActionExpandListenerImplementor, Xamarin.Android.Support.Compat", MenuItemCompat_OnActionExpandListenerImplementor.class, __md_methods);
>>>>>>> Stashed changes
	}


	public MenuItemCompat_OnActionExpandListenerImplementor ()
	{
		super ();
		if (getClass () == MenuItemCompat_OnActionExpandListenerImplementor.class)
<<<<<<< Updated upstream
			mono.android.TypeManager.Activate ("Android.Support.V4.View.MenuItemCompat+IOnActionExpandListenerImplementor, Xamarin.Android.Support.Compat, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
=======
			mono.android.TypeManager.Activate ("Android.Support.V4.View.MenuItemCompat+IOnActionExpandListenerImplementor, Xamarin.Android.Support.Compat", "", this, new java.lang.Object[] {  });
>>>>>>> Stashed changes
	}


	public boolean onMenuItemActionCollapse (android.view.MenuItem p0)
	{
		return n_onMenuItemActionCollapse (p0);
	}

	private native boolean n_onMenuItemActionCollapse (android.view.MenuItem p0);


	public boolean onMenuItemActionExpand (android.view.MenuItem p0)
	{
		return n_onMenuItemActionExpand (p0);
	}

	private native boolean n_onMenuItemActionExpand (android.view.MenuItem p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
