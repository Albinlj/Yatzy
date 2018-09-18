public class Category {
    public string Name { get; }
    public int Val { get; set; }
    public bool IsScored { get; set; }

    public Category(string name) {
        Name = name;
        Val = 0;
        IsScored = false;
    }
}