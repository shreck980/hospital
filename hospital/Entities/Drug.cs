namespace hospital.Entities
{
    public class Drug
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Instruction { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsSelected {  get; set; }
        public Drug(long id, string name, string instruction, DateTime expirationDate)
        {
            Id = id;
            Name = name;
            Instruction = instruction;
            ExpirationDate = expirationDate;
            IsSelected = false;
        }
        public Drug( string name, string instruction, DateTime expirationDate)
        {
            Name = name;
            Instruction = instruction;
            ExpirationDate = expirationDate;
            IsSelected = false;
        }

        public Drug()
        {
            Id = 0;
            Name = "";
            Instruction = "";
            ExpirationDate = DateTime.MinValue;
            IsSelected = false;
        }
    }
}
