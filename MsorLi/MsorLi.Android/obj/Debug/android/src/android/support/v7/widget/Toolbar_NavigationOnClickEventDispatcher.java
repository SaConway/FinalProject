package android.support.v7.widget;


public class Toolbar_NavigationOnClickEventDispatcher
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.view.View.OnClickListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onClick:(Landroid/view/View;)V:GetOnClick_Landroid_view_View_Handler:Android.Views.View/IOnClickListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
<<<<<<< Updated upstream
		mono.android.Runtime.register ("Android.Support.V7.Widget.Toolbar+NavigationOnClickEventDispatcher, Xamarin.Android.Support.v7.AppCompat, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", Toolbar_NavigationOnClickEventDispatcher.class, __md_methods);
=======
		mono.android.Runtime.register ("Android.Support.V7.Widget.Toolbar+NavigationOnClickEventDispatcher, Xamarin.Android.Support.v7.AppCompat", Toolbar_NavigationOnClickEventDispatcher.class, __md_methods);
>>>>>>> Stashed changes
	}


	public Toolbar_NavigationOnClickEventDispatcher ()
	{
		super ();
		if (getClass () == Toolbar_NavigationOnClickEventDispatcher.class)
<<<<<<< Updated upstream
			mono.android.TypeManager.Activate ("Android.Support.V7.Widget.Toolbar+NavigationOnClickEventDispatcher, Xamarin.Android.Support.v7.AppCompat, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
=======
			mono.android.TypeManager.Activate ("Android.Support.V7.Widget.Toolbar+NavigationOnClickEventDispatcher, Xamarin.Android.Support.v7.AppCompat", "", this, new java.lang.Object[] {  });
>>>>>>> Stashed changes
	}

	public Toolbar_NavigationOnClickEventDispatcher (android.support.v7.widget.Toolbar p0)
	{
		super ();
		if (getClass () == Toolbar_NavigationOnClickEventDispatcher.class)
<<<<<<< Updated upstream
			mono.android.TypeManager.Activate ("Android.Support.V7.Widget.Toolbar+NavigationOnClickEventDispatcher, Xamarin.Android.Support.v7.AppCompat, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Support.V7.Widget.Toolbar, Xamarin.Android.Support.v7.AppCompat, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0 });
=======
			mono.android.TypeManager.Activate ("Android.Support.V7.Widget.Toolbar+NavigationOnClickEventDispatcher, Xamarin.Android.Support.v7.AppCompat", "Android.Support.V7.Widget.Toolbar, Xamarin.Android.Support.v7.AppCompat", this, new java.lang.Object[] { p0 });
>>>>>>> Stashed changes
	}


	public void onClick (android.view.View p0)
	{
		n_onClick (p0);
	}

	private native void n_onClick (android.view.View p0);

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
