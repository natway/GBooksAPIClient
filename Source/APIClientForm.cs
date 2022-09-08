using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;

namespace WinFormsApp1
{
    public partial class APIClientForm : Form
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly int portNum = -1;
        private static string? titleInput = "";
        private static string? authorsInput = "";

        public APIClientForm()
        {
            InitializeComponent();
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //Title
            titleInput = textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            //authors
            authorsInput = textBox2.Text;
        }


        private async void button1_Click(object sender, EventArgs e)
        {
            // button
           
            label1.Text = "Working...";
            try
            {   
                await ProcessVolumes(titleInput!, authorsInput!);
            }
            catch (Exception ex)
            {
                if (ex is System.ArgumentNullException)
                {
                    MessageBox.Show("No Results.", "Empty set");
                    label1.Text = "";
                }
                else if (ex is System.Net.Http.HttpRequestException)
                {
                    MessageBox.Show("Enter title or author search terms.", "Invalid input");
                    label1.Text = "";
                }
                else
                {
                    MessageBox.Show(ex.Message, "Exception");
                    label1.Text = "";
                }
            }
        }

        private  async Task ProcessVolumes(string titleInput, string authorsInput)
        {
            int itemsTotal = 0;
            Root? results = new Root();
            List<Item> items = new List<Item>();
            List<Uri> uris = new List<Uri>();
            string titleTerm = titleInput;
            string authorsTerm = authorsInput;

            // establish header
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/Json"));
            client.DefaultRequestHeaders.Add("User-Agent", "Books API Lookup Client");

            // Call API 0th time
            var streamTask = client.GetStreamAsync(GetRequestUri(titleTerm, authorsTerm, 0));
            results = await JsonSerializer.DeserializeAsync<Root>(await streamTask);
            itemsTotal = results!.totalItems;
            items.AddRange(results.items!);

            // Find max iteration count for progress visual
            string iterations = (itemsTotal / 40) < 25 ? (itemsTotal / 40).ToString() : 25.ToString();
            // call API for all URIs for results over 40
            for (int i = 1; i < (itemsTotal / 40) + 1 && i < 25; ++i)
            {
                // Slowest part of program - displays progress visual to user
                label1.Text = $"Working...   {i}/{iterations}";

                Uri uri = GetRequestUri(titleTerm, authorsTerm, i * 40);
                streamTask = client.GetStreamAsync(uri);
                results = await JsonSerializer.DeserializeAsync<Root>(await streamTask);
                try
                {
                    items.AddRange(results!.items!);
                }
                catch {/* null reference */}
            }

            // Remove Duplicates : with either implementation below, i get stackoverflow exception when Equals() calls GetHashCode()
            // List<Item> dedupedVolumes = new HashSet<Item>(volumes).ToList();

            //volSet = new HashSet<Item>(itemArray);
            //items = volSet.ToList();

            // may be best to use LINQ for sort, remove duplicates and display to user
            // but since LINQ also relies on Equals() -> GetHashCode() I used other ways

            // sort list
            items.Sort();

            // remove duplicates in sorted list
            try
            {
                for (int i = 1; i < items.Count - 1; ++i)
                {
                    if (items.ElementAt(i).volumeInfo!.CompareTo(items.ElementAt(i - 1).volumeInfo) == 0)
                    {
                        items.RemoveAt(i);
                    }
                    if (items.ElementAt(i).volumeInfo!.CompareTo(items.ElementAt(i + 1).volumeInfo) == 0)
                    {
                        items.RemoveAt(i + 1);
                    }
                    if (items.ElementAt(i).volumeInfo!.CompareTo(items.ElementAt(i - 1).volumeInfo) == 0)
                    {
                        items.RemoveAt(i);
                    }
                    if (items.ElementAt(i).volumeInfo!.CompareTo(items.ElementAt(i + 1).volumeInfo) == 0)
                    {
                        items.RemoveAt(i + 1);
                    }
                }
            }catch {/* index out of range*/}

            // display results
            listView1.Items.Clear();
            int x = 1;
            foreach (Item it in items)
            {
                try
                {
                    // populate ListView
                    ListViewItem listViewItem = new ListViewItem($"{x}", 0);
                    listViewItem.SubItems.Add($"{it.volumeInfo!.averageRating}");
                    if (it.volumeInfo.getYear() != 0)
                        listViewItem.SubItems.Add($"{it.volumeInfo.getYear()}");
                    else
                        listViewItem.SubItems.Add("");
                    listViewItem.SubItems.Add($"{it.volumeInfo.listAuthors()}");
                    listViewItem.SubItems.Add($"{it.volumeInfo.title}");
                    if(!String.IsNullOrEmpty(it.volumeInfo.listAuthors()) && !String.IsNullOrEmpty(it.volumeInfo.title))
                    listView1.Items.Add(listViewItem);
                }
                catch
                {/* null reference */}
                x++;
            }
            label1.Text = "";
        }

        private static Uri GetRequestUri(string? title, string? author, int iteration)
        {
            UriBuilder uriBuilder = new UriBuilder(scheme: "https", host: "www.googleapis.com", port: portNum, path: "/books/v1/volumes",
                extraValue: MakeQueryString(title, author, iteration));
            return uriBuilder!.Uri;
        }

        private static String? MakeQueryString(string? title, string? author, int page)
        {  
            // Create query string

            string query = "?q=";
            bool titleExists = false;

            if (!string.IsNullOrEmpty(title))
            {
                title = Uri.EscapeDataString(title);
                query = query + $"intitle:{title}";
                titleExists = true;
            }

            if (!string.IsNullOrEmpty(author))
            {
                author = Uri.EscapeDataString(author);
                if (titleExists)
                {
                    query = query + $"+inauthor:{author}";
                }
                else
                {
                    query = query + $"inauthor:{author}";
                }
            }
            query = query + $"&filter=paid-ebooks&maxResults=40&startIndex={page}";
            return query;
        }
    }
}