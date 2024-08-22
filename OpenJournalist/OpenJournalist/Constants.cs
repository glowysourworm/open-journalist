using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OpenJournalist
{
    public enum PlatformType
    {
        [Display(Name = "Local Database")]
        [Description("Database or local data store. See configuration to setup local database.")]
        LocalDB = 0,

        [Display(Name = "Youtube")]
        [Description("The social media platform Youtube. See configuration to understand this service connection.")]
        Youtube = 1,

        [Display(Name = "Rumble")]
        [Description("The social media platform Rumble. See configuration to understand this service connection.")]
        Rumble = 2
    }
}
