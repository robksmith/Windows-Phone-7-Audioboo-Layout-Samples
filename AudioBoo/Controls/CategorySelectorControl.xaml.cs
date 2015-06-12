
#region Usings

using AudioBoo.Models;
using AudioBoo.ViewModels;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

#endregion

namespace AudioBoo.Controls
{
    public partial class CategorySelectorControl : UserControl
    {
        public class MyCategoryBinding
        {
            public int CategoryId { get; set; }
            public string Name { get; set; }
            public Uri Image { get; set; }
        }

        #region Fields

        public event EventHandler<EventArgs> CategoryPressed;
        private CategoryData category;

        #endregion

        #region Properties

        public string NoSelectionMadeText { get; set; }
        public CategoryData Category { get { return category; } private set { } }

        #endregion


        #region Constructors

        public CategorySelectorControl()
        {
            InitializeComponent();

            CategorySelector.Tap += CategorySelect_Tap;

            //LayoutRoot.ManipulationStarted += Animation.Standard_ManipulationStarted_1;
            //LayoutRoot.ManipulationCompleted += Animation.Standard_ManipulationCompleted_1;
        }

        #endregion


        #region Event Handlers

        void CategorySelect_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (CategoryPressed != null)
            {
                CategoryPressed(this, new EventArgs());
            }
        }

        #endregion


        #region Helpers

        internal void Refresh(CategoryData category)
        {
            this.category = category;

            if (category != null)
            {
                //var cats = App.ViewModel.CategoriesViewModel.CategoryList;
                //CategoryData category = (from cat in cats
                //                         where cat.CategoryId == catId
                //                         select cat).FirstOrDefault();

                var toBind = new MyCategoryBinding() { CategoryId = category.CategoryId, Name = category.Title, Image = category.CategoryIcon};
                LayoutRoot.DataContext = toBind;

                TextBlockDescription.Foreground = App.AppConstants.NormalTextColourBrush;
            }
            else
            {
                var toBind = new MyCategoryBinding() { CategoryId = 0, Name = NoSelectionMadeText, Image = new Uri("../Images/NotChosen.png", UriKind.Relative) };
                LayoutRoot.DataContext = toBind;

                TextBlockDescription.Foreground = App.AppConstants.WatermarkTextColourBrush;
            }
        }

        #endregion


        internal string SelectedId()
        {
            if (category != null)
            {
                return category.CategoryId.ToString();
            }
            return "0";
        }

    }
}
