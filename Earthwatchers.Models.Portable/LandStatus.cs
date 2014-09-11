namespace Earthwatchers.Models
{
    public enum LandStatus
    {
        Unwatched=0, // land that will not be watched after anymore, it's water for example
        Unassigned=1, // initial state: land thats available for assignment
        NotChecked=2, // land thats assigned but not yet checked
        Ok=3, // land thats assigned and ok
        Alert=4 // land with deforestation going on according to the owner
    }
}
