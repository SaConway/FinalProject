package md5b5a51392fe8815462cedec90408c1d53;


public class FloatingActionButtonRenderer
	extends md5270abb39e60627f0f200893b490a1ade.ViewRenderer_2
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onLayout:(ZIIII)V:GetOnLayout_ZIIIIHandler\n" +
			"";
		mono.android.Runtime.register ("SuaveControls.FloatingActionButton.Droid.Renderers.FloatingActionButtonRenderer, SuaveControls.FloatingActionButton.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", FloatingActionButtonRenderer.class, __md_methods);
	}


	public FloatingActionButtonRenderer (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == FloatingActionButtonRenderer.class)
			mono.android.TypeManager.Activate ("SuaveControls.FloatingActionButton.Droid.Renderers.FloatingActionButtonRenderer, SuaveControls.FloatingActionButton.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.Util.IAttributeSet, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:System.Int32, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public FloatingActionButtonRenderer (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == FloatingActionButtonRenderer.class)
			mono.android.TypeManager.Activate ("SuaveControls.FloatingActionButton.Droid.Renderers.FloatingActionButtonRenderer, SuaveControls.FloatingActionButton.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.Util.IAttributeSet, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0, p1 });
	}


	public FloatingActionButtonRenderer (android.content.Context p0)
	{
		super (p0);
		if (getClass () == FloatingActionButtonRenderer.class)
			mono.android.TypeManager.Activate ("SuaveControls.FloatingActionButton.Droid.Renderers.FloatingActionButtonRenderer, SuaveControls.FloatingActionButton.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}


	public void onLayout (boolean p0, int p1, int p2, int p3, int p4)
	{
		n_onLayout (p0, p1, p2, p3, p4);
	}

	private native void n_onLayout (boolean p0, int p1, int p2, int p3, int p4);

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
