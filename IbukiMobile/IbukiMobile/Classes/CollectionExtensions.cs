#nullable enable
using System.Collections.Generic;
using System.Linq;
using IbukiBooruLibrary.Classes;

namespace IbukiMobile.Classes; 

// Both methods have the same execution time
public static class BooruCollectionExtensions {
    public static BooruPost? GetByID(this IEnumerable<BooruPost> collection, int ID) {
        // Convert collection to dictionary with ID's
        Dictionary<int, BooruPost> dict = collection.ToDictionary(post => post.ID, post => post);
        
        // Find item in dictionary
        return dict.TryGetValue(ID, out BooruPost foundItem) ? foundItem : null;
    }
    
    public static BooruPost? BinaryGetByID(this IEnumerable<BooruPost> collection, int ID) {
        // Sort the collection by ID 
        // Questionable, since it SHOULD be sorted by ID anyway...
        List<BooruPost> sorted = collection.OrderBy(post => post.ID).ToList();
        
        // Find item place in sorted collection
        int index = sorted.BinarySearch(new BooruPost(ID));
        return index != -1 ? sorted[index] : null;
    }
    
}