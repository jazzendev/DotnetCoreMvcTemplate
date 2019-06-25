using System.Collections.Generic;
using Newtonsoft.Json;

namespace MyTemplate.Core.Repository
{
    public class PaginationQuery : IPaginationQuery
    {
        public int Page { get; set; } = 0;
        public int Size { get; set; } = DefaultSettings.DefaultPageSize;
        public long Offset { get { return this.Page * this.Size; } }

        private string _sortItemString;
        private List<SortingItem> _sortingItems;
        public string SortItemString
        {
            get
            {
                return _sortItemString;
            }
            set
            {
                _sortItemString = value;
                _sortingItems = JsonConvert.DeserializeObject<List<SortingItem>>(_sortItemString);
            }
        }

        public List<SortingItem> SortingItems
        {
            get
            {
                return _sortingItems;
            }
            set
            {
                _sortingItems = value;
                _sortItemString = JsonConvert.SerializeObject(_sortingItems);
            }
        }
    }

    public class SimplePaginationQuery : PaginationQuery
    {
        public bool? IsValid { get; set; }
        public string Name { get; set; }
    }

    public class PaginationResult<T> : IPaginationResult<T>
    {
        public PaginationQuery Query { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public long Total { get; set; }
        public IEnumerable<T> Data { get; set; }
    }

    public class SortingItem
    {
        public string Name { get; set; }
        public bool IsAscending { get; set; }
    }
}
