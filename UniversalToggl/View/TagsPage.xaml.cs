﻿using System.Collections.ObjectModel;
using System.Linq;
using TogglAPI;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UniversalToggl.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TagsPage : Page
    {
        public ObservableCollection<Tag> Tags
        {
            get { return (ObservableCollection<Tag>) App.Data.Tags.Distinct(new TagNameComparer()); }
        }

        public TagsPage()
        {
            this.InitializeComponent();
        }
    }
}
