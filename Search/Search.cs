namespace ff.Search;
/// <summary>
/// 1. triggered at bottom with the '>' char('>' is not allowed in file/dir name)
///    1. '> ' glob search
///    1. '>r ' regex search
///    1. '>l ' logic search
/// 1. by default search the file/dir name
/// 1. by default glob: ? * search 
/// 1. button for regex search
/// 1. button for logic search(&|())
///    * regex(abc)&file(fileName)|glob()
/// 1. case-sensitive and whole world match for all kinds of search
/// 1. '`' as escaping char
/// 1. text search support(take on regex string), (auto sort it to last for all &ed filters)
///
/// ## note:
/// 1. for filer take an array of string, use '|' as separator(it is not allowed in file name), if the string contains '|' please use '`' to escape the char.
/// </summary>
internal class Search
{
}