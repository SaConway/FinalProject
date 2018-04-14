<<<<<<< HEAD
﻿using MsorLi.Views;
using System;
using System.Collections.ObjectModel;
=======
﻿using MsorLi.Utilities;
using MsorLi.Views;
>>>>>>> 10a6c45... idan profile
using Xamarin.Forms;

namespace MsorLi
{
    public partial class App : Application
	{
        public App ()
		{
            InitializeComponent();
            MainPage = new NavigationPage(new ItemListPage());
        }
	}
}