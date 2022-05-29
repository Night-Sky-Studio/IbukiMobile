/**
 * This is an example file that is used in the final software
 *
 * In order for Ibuki to work with your extension script
 * you have to define an extension object with all the information
 *
 * Sadly, due to the fact that Jint library does not support async/await
 * or Promises in general (.then() chaining), all the networking capabilities
 * are handled internally by Ibuki and thus are not exposed to the Extensions
 * 
 * Currently, the problem of networking is solved by providing endpoint URLs in
 * Extension configuration
 * 
 * You can add whatever internal functions for your parser extension
 *
 * Ibuki will call the following functions, this functions are mandatory to implement:
 * - {@link GetPostsURL}
 * - {@link GetPostURL}
 * - {@link GetUserFavoritesURL}
 * - {@link GetPostChildrenURL}
 * 
 * You can either implement JSON or XML type parsers, but you can't implement both
 * Also, the parser's type should be defined in the Extension configuration
 * 
 * JSON functions: 
 * - {@link ParsePostsJSON}
 * - {@link ParseErrorJSON}
 * 
 * XML functions:
 * - {@link ParsePostXML}
 * - {@link ParseErrorXML}
 *
 * The order of parameters in these functions should not be changed!
 */
const IbukiBooruLibrary = importNamespace("IbukiBooruLibrary.API");

const Extension = {
    name: "Danbooru",
    api_type: "json",
    base_url: "https://danbooru.donmai.us",
    tags_separator: "_",
}

function GetPostsURL(page = 1, limit = 20, search = "", auth = "") {
    let url = new IbukiBooruLibrary.URL(Extension.base_url, "/posts.json")
    
    // Mandatory params
    url.AppendQueryParam("page", `${page}`)
    url.AppendQueryParam("limit", `${limit}`)
    
    // Optional params
    if (search !== "") url.AppendQueryParam("tags", search)
    if (auth !== "") url.AppendString(auth)
    
    return url.ToString()
}

function GetPostURL(id) {
    let url = new IbukiBooruLibrary.URL(Extension.base_url, `/posts/${id}.json`)
    return url.ToString()
}

function GetUserFavoritesURL(page = 1, limit = 20, username = "", auth = "") {
    return GetPostsURL(page, limit, `ordfav:${username}`, auth)
}

function GetPostChildrenURL(id, auth = "") {
    return GetPostsURL(1, 200, `parent:${id} -id:${id}`, auth)
}

function MakeTagsFromTagsString(string, separator) {
    let result = []
    let array = string.split(separator)
    
    for (let i = 0; i < array.length; i++) {
        result.push(array[i])
    }
    
    return result
}

function ParsePostJSON(json) {
    if (typeof(json) !== typeof(JSON)) json = JSON.parse(json)
    
    // When the required by Ibuki fields are empty - don't add this post to the returnable array
    if (json.id == undefined || json.preview_file_url == undefined || json.large_file_url == undefined || json.is_deleted == true || json.is_banned == true) 
        return null
    
    return {
        ID: json.id,
        PreviewFileURL: json.preview_file_url,
        LargeFileURL: json.large_file_url,
        DirectURL: GetPostURL(json.id).replace(".json", ""),
        Tags: {
            CopyrightTags: MakeTagsFromTagsString(json.tag_string_copyright, Extension.tags_separator),
            CharacterTags: MakeTagsFromTagsString(json.tag_string_character, Extension.tags_separator),
            SpeciesTags: null,
            ArtistTags: MakeTagsFromTagsString(json.tag_string_artist, Extension.tags_separator),
            LoreTags: null,
            GeneralTags: MakeTagsFromTagsString(json.tag_string_general, Extension.tags_separator),
            MetaTags: MakeTagsFromTagsString(json.tag_string_meta, Extension.tags_separator)
        },
        Information: {
            UploaderID: json.uploader_id,
            Score: {
                UpVotes: json.up_score,
                DownVotes: json.down_score,
                FavoritesCount: json.fav_count,
            },
            Source: json.source,
            ParentID: json.parent_id,
            HasChildren: json.has_children,
            CreatedAt: json.created_at,
            UploadedAt: json.updated_at,
            FileExtension: json.file_ext,
            FileSize: json.file_size,
            ImageWidth: json.image_width,
            ImageHeight: json.image_height,
        }
    }
}

/**
 * Parses received from Ibuki JSON string
 * 
 * @param json_string JSON response 
 * @returns {Object[]} Array of parsed BooruImage objects
 * @remarks Internally called by Ibuki
 */
function ParsePostsJSON(json_string) {
    let result = []
    let array = JSON.parse(json_string)
    
    for (let i = 0; i < array.length; i++) {
        let post = ParsePostJSON(array[i])
        if (post != null)
            result.push(post)
    }   
    
    return result
}

function ParseErrorJSON(json_string) {
    let response = JSON.parse(json_string)
    return { error: response.error, message: response.message }
}

function ParsePostXML(xml_string) {
    
}