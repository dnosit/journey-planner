namespace JourneyPlanner
{
    public class Journey
    {
        public List<Location> Locations { get; set; } = new List<Location>();   // Locations (ascending) used by this journey 

        public int TotalDistanceKm { get; set; }

        public double DesirabilityValue { get; set; }



        public int GetNumStops() { return Locations.Count; } // Number of locations used by this journey 

         

    }




}
