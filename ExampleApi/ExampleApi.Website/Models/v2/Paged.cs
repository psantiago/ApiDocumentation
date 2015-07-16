using System.Collections;
using System.Collections.Generic;

namespace ExampleApi.Website.Models.v2
{
    public class Paged<T>
    {
        public Paged(IEnumerable<T> items, string nextPage, string currentPage, string previousPage, int itemsPerPage, int page, int total)
        {
            Items = items;
            NextPage = nextPage;
            CurrentPage = currentPage;
            PreviousPage = previousPage;
            ItemsPerPage = itemsPerPage;
            Page = page;
            Total = total;
        }

        /// <summary>
        /// This is the total number of items, regardless of paging.
        /// If this is some sort of paged search data, 
        /// this is the total number that the search returns (regardless of paging)
        /// </summary>
        public int Total { get; private set; }
        public int Page { get; private set; }
        public int ItemsPerPage { get; private set; }
        public string PreviousPage { get; private set; }
        public string CurrentPage { get; private set; }
        public string NextPage { get; private set; }

        /// <summary>
        /// The items of the current page to return.
        /// </summary>
        public IEnumerable<T> Items { get; private set; }
    }
}