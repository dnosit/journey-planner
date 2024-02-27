namespace JourneyPlanner
{
    public class Location
    {
        public int LocationId { get; set; } 

        public int TripsTotal { get; set; } // Sum total number of stops in each Journey passing through this location 

        public int NumTripStops { get; set; } // Sum of times this location is used by a Journey 

        // public string LocationName { get; set; }   

        // Location NextLocationInSequence { get; set; } 




    }

}
