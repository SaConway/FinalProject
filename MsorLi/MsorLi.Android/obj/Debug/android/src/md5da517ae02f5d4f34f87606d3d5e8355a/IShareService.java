package md5da517ae02f5d4f34f87606d3d5e8355a;


public class IShareService
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("MsorLi.Droid.IShareService, MsorLi.Android", IShareService.class, __md_methods);
	}


	public IShareService ()
	{
		super ();
		if (getClass () == IShareService.class)
			mono.android.TypeManager.Activate ("MsorLi.Droid.IShareService, MsorLi.Android", "", this, new java.lang.Object[] {  });
	}

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
