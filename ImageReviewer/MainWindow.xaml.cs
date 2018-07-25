using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ImageReviewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BitmapImage bitmapImage;
        string SelectedPath;
        Utilities Utils;

        public MainWindow()
        {
            InitializeComponent();
            EventManager.RegisterClassHandler(typeof(ListBox), Keyboard.KeyDownEvent, new KeyEventHandler(keyDown), true);
            EventManager.RegisterClassHandler(typeof(TextBox), Keyboard.KeyDownEvent, new KeyEventHandler(keyDown), true);
        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.Up)
            {
                if (this.list_files.SelectedIndex > 0)
                {
                    this.list_files.SelectedIndex -= 1;
                }
                else
                {
                    this.list_files.SelectedIndex = this.list_files.Items.Count - 1;
                }
                return;
            }

            if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.Down)
            {
                if (this.list_files.SelectedIndex != this.list_files.Items.Count - 1)
                {
                    this.list_files.SelectedIndex += 1;
                }
                else
                {
                    this.list_files.SelectedIndex = 0;
                }

                return;
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs eventArgs)
        {
            Utils = new Utilities();
        }

        private void btn_select_folder_click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            var dialogReturn = dialog.ShowDialog(this);    
            
            if (dialogReturn == true)
            {
                this.txt_source_folder.Text = dialog.SelectedPath;
                this.SelectedPath = dialog.SelectedPath;

                var fileList = Utils.GetFileList(dialog.SelectedPath);

                this.list_files.Items.Clear();

                int x = 0;
                foreach (var file in fileList)
                {
                    Utils.CheckDatabaseForFile(file.Name, dialog.SelectedPath);
                    Utils.Log(String.Format("Loading file #{0}: {1}", x++, file.Name), Utilities.LoggingLevels.Information);
                    this.list_files.Items.Add(file.Name);
                }
            }
        }

        private void list_files_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dir = this.txt_source_folder.Text;

            try
            {
                var file = this.list_files.SelectedItem.ToString();

                if (file is null) { return; }

                if (bitmapImage != null) { bitmapImage = null; }
                bitmapImage = new BitmapImage(new Uri(String.Format(@"{0}\{1}", dir, file)));
                bitmapImage.DecodePixelHeight = Convert.ToInt16(bitmapImage.PixelHeight * .5) > this.tagging_image.Height ? Convert.ToInt32(this.tagging_image.Height) : Convert.ToInt32(bitmapImage.PixelHeight * .5);
                bitmapImage.DecodePixelWidth = Convert.ToInt16(bitmapImage.PixelWidth * 5) > this.tagging_image.Width ? Convert.ToInt32(this.tagging_image.Width) : Convert.ToInt32(bitmapImage.PixelWidth * .5);

                this.tagging_image.Source = bitmapImage;
                this.entry_tag_name.IsEnabled = true;
                int imageId = Utils.GetExistingImageId(String.Format(@"{0}\{1}", this.SelectedPath, file));
                this.refresh_image_tag_list(imageId);       
            }
            catch (Exception exc)
            {
                Utils.Log(String.Format("Exception loading file: {0}", exc.Message), Utilities.LoggingLevels.Error);
            }
        }

        private void text_tag_insert_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Enter)
            {
                int tagId;

                if (Utils.CheckTagExists(this.entry_tag_name.Text))
                {
                    tagId = Utils.GetExistingTagId(this.entry_tag_name.Text);
                }
                else
                {
                    tagId = Utils.CreateNewTag(this.entry_tag_name.Text);
                }
                  
                int imageId = Utils.GetExistingImageId(String.Format(@"{0}\{1}", this.SelectedPath, this.list_files.SelectedItem.ToString()));
                if (Utils.AddImageTag(tagId, imageId))
                {
                    this.entry_tag_name.Text = "";
                    this.refresh_image_tag_list(imageId);
                    this.refresh_pool_tag_list();
                }
            }
        }

        private void refresh_pool_tag_list()
        {
            this.list_tag_pool.Items.Clear();

            var tags = Utils.GetTags();
            
            foreach (var t in tags)
            {
                this.list_tag_pool.Items.Add(t.Name);
            }
        }

        private void refresh_image_tag_list(int imageId)
        {
            this.list_image_tags.Items.Clear();
            var tags = Utils.GetTags(imageId);

            foreach (var t in tags)
            {
                this.list_image_tags.Items.Add(t.Name);
            }
        }

        private void list_tag_pool_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            if (listBox.SelectedIndex == -1) { return; }

            if (Keyboard.Modifiers == ModifierKeys.Control)
            {                
                string tagName = (string) listBox.SelectedValue;
                int tagId = Utils.GetExistingTagId(tagName);
                int imageId = Utils.GetExistingImageId(String.Format(@"{0}\{1}", this.SelectedPath, this.list_files.SelectedItem.ToString()));

                Utils.AddImageTag(tagId, imageId);
                this.refresh_image_tag_list(imageId);
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                string tagName = (string)listBox.SelectedValue;
                int tagId = Utils.GetExistingTagId(tagName);

                var mbox = MessageBox.Show(
                    String.Format("Are you sure you want to delete tag: {0}", tagName), "Confirm Delete", MessageBoxButton.OKCancel);

                if (mbox == MessageBoxResult.OK)
                {
                    Utils.DeleteExistingTag(tagId);
                    listBox.Items.RemoveAt(listBox.SelectedIndex);
                }
            }
        }

        private void list_tag_pool_Loaded(object sender, RoutedEventArgs e)
        {
            var listbox = (ListBox)sender;
            listbox.Items.Clear();

            var tags = Utils.GetTags();
            foreach (var tag in tags)
            {
                listbox.Items.Add(tag.Name);
            }
        }

        private void list_image_tags_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                System.Windows.Controls.ListBox listBox = (System.Windows.Controls.ListBox)sender;       
                
                if (listBox.SelectedIndex == -1) { return; }
                string tagName = (string)listBox.SelectedValue;
                

                int tagId = Utils.GetExistingTagId(tagName);
                int imageId = Utils.GetExistingImageId(String.Format(@"{0}\{1}", this.SelectedPath, this.list_files.SelectedItem.ToString()));
                                
                Utils.RemoveImageTag(tagId, imageId);
                this.refresh_image_tag_list(imageId);
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Height != 0 && e.PreviousSize.Width != 0)
            {
                var heightDifference = e.NewSize.Height / e.PreviousSize.Height;
                var widthDifference = e.NewSize.Width / e.PreviousSize.Width;

                this.tagging_image.Width *= widthDifference;
                this.tagging_image.Height *= heightDifference;

                this.review_image.Width *= widthDifference;
                this.review_image.Height *= heightDifference;
            }
        }

        private void entry_tag_name_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;            
            var text = textBox.Text;

            var changes = e.Changes.GetEnumerator();
            changes.MoveNext();

            if (text.Length >= 2 && changes.Current.RemovedLength == 0)
            {
                var originalLength = text.Length;
                var suggestedText = Utils.GetFirstTagText(text);

                if (suggestedText != String.Empty)
                {
                    textBox.Text = suggestedText;

                    textBox.CaretIndex = originalLength;
                    textBox.SelectionStart = originalLength;
                    textBox.SelectionLength = suggestedText.Length - originalLength;
                }                
            }
        }

        private void review_lb_select_tags_Loaded(object sender, RoutedEventArgs e)
        {
            var listbox = (ListBox)sender;
            listbox.Items.Clear();

            var tags = Utils.GetTags();
            foreach (var tag in tags)
            {
                listbox.Items.Add(tag.Name);
            }
        }

        private void review_lb_applied_tags_Loaded(object sender, RoutedEventArgs e)
        {
            var lb = (ListBox)sender;
            ((INotifyCollectionChanged)lb.Items).CollectionChanged += review_lb_applied_tags_CollectionChanged;                        
        }

        private void review_lb_applied_tags_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            List<string> files = Utils.GetMatchingFileList(this.review_lb_applied_tags.Items, this.review_lb_ignore_tags.Items);

            this.review_lb_file_list.Items.Clear();

            if (files.Count == 0) { return; }

            foreach (var file in files)
            {
                this.review_lb_file_list.Items.Add(file);
            }
        }

        private void review_lb_select_tags_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                ListBox listBox = ((ListBox)sender);

                if (listBox.SelectedIndex == -1) { return; }
                string tagName = (string)listBox.SelectedValue;

                if (!this.review_lb_applied_tags.Items.Contains(tagName))
                {
                    this.review_lb_applied_tags.Items.Add((string)tagName);
                }
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                ListBox listBox = ((ListBox)sender);

                if (listBox.SelectedIndex == -1) { return; }
                string tagName = (string)listBox.SelectedValue;

                if (!this.review_lb_applied_tags.Items.Contains(tagName))
                {
                    this.review_lb_ignore_tags.Items.Add((string)tagName);
                }
            }

        }

        private void review_lb_file_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
            try
            {
                ListBox listBox = (ListBox)sender;

                var file = listBox.SelectedItem.ToString();
                if (file == String.Empty) { return; }

                if (bitmapImage != null) { bitmapImage = null; }
                bitmapImage = new BitmapImage(new Uri(file));
                bitmapImage.DecodePixelHeight = Convert.ToInt32(bitmapImage.PixelHeight * .5) > this.review_image.Height ? Convert.ToInt32(this.review_image.Height) : Convert.ToInt32(bitmapImage.PixelHeight * .5);
                bitmapImage.DecodePixelWidth = Convert.ToInt32(bitmapImage.PixelWidth * 5) > this.review_image.Width ? Convert.ToInt32(this.review_image.Width) : Convert.ToInt32(bitmapImage.PixelWidth * .5);

                this.review_image.Source = bitmapImage;
                this.entry_tag_name.IsEnabled = true;

                var imageid = Utils.GetExistingImageId(file);
                var tags = Utils.GetTags(imageid);

                this.review_lb_selected_image_tags.Items.Clear();
                foreach (var tag in tags)
                {
                    this.review_lb_selected_image_tags.Items.Add(tag.Name);
                }
            }
            catch (Exception exc)
            {
                Utils.Log(String.Format("Exception loading file: {0}", exc.Message), Utilities.LoggingLevels.Error);
            }
        }

        private void review_btn_clear_Click(object sender, RoutedEventArgs e)
        {
            this.review_lb_select_tags.Items.Clear();
            var tags = Utils.GetTags();
            foreach (var tag in tags)
            {
                this.review_lb_select_tags.Items.Add(tag.Name);
            }

            this.review_lb_applied_tags.Items.Clear();
            this.review_lb_selected_image_tags.Items.Clear();
            this.review_lb_ignore_tags.Items.Clear();
            this.review_image.Source = null;
        }

        private void review_lb_applied_tags_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                System.Windows.Controls.ListBox listBox = (System.Windows.Controls.ListBox)sender;
                if (listBox.SelectedIndex == -1) { return; }
                listBox.Items.RemoveAt(listBox.SelectedIndex);
            }
        }

        private void review_lb_selected_image_tags_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                System.Windows.Controls.ListBox listBox = (System.Windows.Controls.ListBox)sender;
                if (listBox.SelectedIndex == -1) { return; }

                var tagname = listBox.SelectedValue;

                if (!this.review_lb_applied_tags.Items.Contains(tagname))
                {
                    this.review_lb_applied_tags.Items.Add(tagname);
                }
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                System.Windows.Controls.ListBox listBox = (System.Windows.Controls.ListBox)sender;
                if (listBox.SelectedIndex == -1) { return; }

                var tagname = listBox.SelectedValue;

                if (!this.review_lb_ignore_tags.Items.Contains(tagname))
                {
                    this.review_lb_ignore_tags.Items.Add(tagname);
                }
            }
        }

        private void review_lb_ignore_tags_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(Keyboard.Modifiers == ModifierKeys.Shift)
            {
                System.Windows.Controls.ListBox listBox = (System.Windows.Controls.ListBox)sender;
                if (listBox.SelectedIndex == -1) { return; }
                listBox.Items.RemoveAt(listBox.SelectedIndex);
            }
        }

        private void review_lb_ignore_tags_Loaded(object sender, RoutedEventArgs e)
        {
            var lb = (ListBox)sender;
            ((INotifyCollectionChanged)lb.Items).CollectionChanged += review_lb_ignore_tags_CollectionChanged;
        }

        private void review_lb_ignore_tags_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            List<string> files = Utils.GetMatchingFileList(this.review_lb_applied_tags.Items, this.review_lb_ignore_tags.Items);

            this.review_lb_file_list.Items.Clear();

            if (files.Count == 0) { return; }

            foreach (var file in files)
            {
                this.review_lb_file_list.Items.Add(file);
            }
        }

        private void tagging_cb_artist_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Control)
            {                
                if (!Utils.CheckArtistExists(tagging_cb_artist.Text, out int artistId))
                {
                    Utils.AddArtist(tagging_cb_artist.Text, out artistId);
                    tagging_cb_artist.Text = "";
                    tagging_cb_artist.SelectedItem = artistId;
                }
                else
                {
                    tagging_cb_artist.SelectedItem = artistId;
                }
            }
        }

        private void tagging_cb_artist_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
