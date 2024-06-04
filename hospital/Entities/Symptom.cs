namespace hospital.Entities
{
    public class Symptom
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public string Description { get; set; }
     
        public Symptom(long id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
            IsSelected = false;
           
        }
        public Symptom()
        {
            Id = 0;
            Name = "";
            Description = "";
            IsSelected = false;

        }
    }
}
