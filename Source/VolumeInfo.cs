using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1
{

    // Class for Volume info - target data
    public class VolumeInfo : IComparable<VolumeInfo>, IEquatable<VolumeInfo>
    {
        public string? title { get; set; }
        public List<string>? authors { get; set; }
        public string? publishedDate { get; set; }
        public double averageRating { get; set; }

        public string? listAuthors()
        {
            string? authorsString = "";
            bool firstEntry = true;
            try
            {
                foreach (string? a in this.authors!)
                {
                    if (firstEntry)
                    {
                        authorsString = a;
                        firstEntry = false;
                    }
                    else
                    {
                        authorsString = authorsString + ", " + a;
                    }
                }
            }
            catch {/* null reference */ }
            return authorsString;
        }

        public int CompareTo(VolumeInfo? other)
        {
            //if title author & year match return 0
            if (title == other!.title &&
                listAuthors()!.ToLower() == other.listAuthors()!.ToLower() &&
                getYear() == other.getYear())
                return 0;
            else if (other!.averageRating > this.averageRating)
                return 1;
            else if (other.averageRating < this.averageRating)
                return -1;
            else if ((int)other.averageRating == (int)this.averageRating)
                try
                {
                    return this.listAuthors()!.ToLower().CompareTo(other.listAuthors());
                }
                catch { return 0; }
            else
                return 1;
        }
        public int getYear()
        {
            DateOnly date;
            if (DateOnly.TryParse(this.publishedDate, out date))
            {
                return date.Year;
            }
            else
                return 0;
        }
        public override bool Equals(object? obj)
        {
            return obj is VolumeInfo info &&
                   title == info.title &&
                   listAuthors()!.ToLower() == info.listAuthors()!.ToLower() &&
                   getYear() == info.getYear();
        }

        public bool Equals(VolumeInfo? other)
        { // volumes are equal if author, title, and year are the same.
            return Equals(other);
        }

        public override int GetHashCode()
        {
            int result = getYear();
            return HashCode.Combine(title, result, listAuthors());
        }
    }

    // Base class returned in JSON
    public class Root
    {
        public string? kind { get; set; }
        public int totalItems { get; set; }
        public List<Item>? items { get; set; }
    }

    // VolumeInfo listed in Items in base class
    public class Item : IComparable<Item>, IEquatable<Item>
    {
        public VolumeInfo? volumeInfo { get; set; }

        public int CompareTo(Item? other)
        {
            int result = 1;
            result = this.volumeInfo!.CompareTo(other!.volumeInfo);
            return result;
        }
        public bool Equals(Item? other)
        {
            return Equals(other);
        }
        public override bool Equals(object? obj)
        {
            return obj is Item item &&
                   EqualityComparer<VolumeInfo>.Default.Equals(volumeInfo, item.volumeInfo);
        }

        public override int GetHashCode()
        => HashCode.Combine(volumeInfo);
    }
}


