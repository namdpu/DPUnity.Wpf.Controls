namespace DPUnity.Wpf.Controls.Controls.ProjectSettingsMenus
{
    public interface IDP_ProjectSetting
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActivated { get; set; }
        public void Save();
    }
}
