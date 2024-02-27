using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace JourneyPlanner
{

    #region Algorithm Interface / Factory  

    public interface IJourneyAlgorithm
    {
        public List<Journey> GetJourneysDesirability(List<Journey> originalList);

    }


    class JourneyPlanner
    {
        private IJourneyAlgorithm Algorithm;
        private readonly JourneyAlgorithmFactory AlgorithmFactory; 

        public JourneyPlanner(JourneyAlgorithmFactory algorithmFactory)
        {
            AlgorithmFactory = algorithmFactory;
        }

        public void SetAlgorithm(string algorithmName)
        {
            Algorithm = AlgorithmFactory.GetJourneyAlgorithm(algorithmName);
        }

        public List<Journey> GetJourneysDesirability(List<Journey> originalList)
        {
            if (Algorithm == null)
            {
                throw new InvalidOperationException("Journey algorithm is not set.");
            }

            return Algorithm.GetJourneysDesirability(originalList);
        }
    }


    public abstract class JourneyAlgorithmCreator
    {
        public abstract IJourneyAlgorithm GetJourneyAlgorithm(string algorithmName);
    }

    public class JourneyAlgorithmFactory : JourneyAlgorithmCreator
    {
        public override IJourneyAlgorithm GetJourneyAlgorithm(string algorithmName)
        {
            switch (algorithmName.ToUpper())
            {
                case "SAM-EMILY": return new JourneyAlgorithmSamEmily();

                case "CO2": return new JourneyAlgorithmCO2Min(); 

                // More Journey algorithms...  

                default: throw new ArgumentException($"Invalid algorithm name. The name; {algorithmName} is not recognised as an implemented option.");
            }
        }
    }

    #endregion 



    #region Algorithm Implementations  

    // Desirability Algorithm - SilverRail 'Sam and Emily' 
    // See; https://silverrailtech.com/knowledge/post/silverrail-university-of-queensland-internship-programme 
    public class JourneyAlgorithmSamEmily : IJourneyAlgorithm
    {
        public List<Journey> GetJourneysDesirability(List<Journey> originalList)
        {
            // Determine our set of locations/stops (ie; distinct) 
            // Also use this to store the Journeys which pass through each location 
            // and other details to calculate totals for all journeys 
            Dictionary<int, List<Journey>> Trips = new Dictionary<int, List<Journey>>();
            Dictionary<int, Location> locations = new Dictionary<int, Location>();

            foreach (Journey journey in originalList)
            {
                foreach (Location loc in journey.Locations)
                {
                    if (Trips.ContainsKey(loc.LocationId) == false)
                    {
                        // Record location and Journey for reference 
                        locations.Add(loc.LocationId, loc);
                        Trips.Add(loc.LocationId, new List<Journey>());
                    }

                    // Update TripsTotal 
                    // ie; add to the sum of locations on this Journey to this location 
                    locations[loc.LocationId].TripsTotal += journey.GetNumStops();
                    // Update NumTripStops 
                    // ie; The count of times this location is used by a Journey 
                    locations[loc.LocationId].NumTripStops += 1; // for this Journey 
                    // Add this Journey to this location's Trips  
                    Trips[loc.LocationId].Add(journey);
                    

                }
            }

            // Now we have all the totals we can calculate the desirability for each Journey 
            foreach (Journey journey in originalList)
            {
                // Final Arithmetic to populate variables specific to this journey  
                int tripsTotal = 0;   
                int numTripStops = 0; // Determine for this location only ? 
                
                foreach(Location loc in journey.Locations)
                {
                    // Need to Retrieve from dictionary instance of location, which has had values updated. 
                    tripsTotal += locations[loc.LocationId].TripsTotal; 
                    numTripStops += locations[loc.LocationId].NumTripStops;  
                }

                // Calculate Desirability now we have all the variables or methods for retreival 
                journey.DesirabilityValue = (numTripStops / journey.GetNumStops() ) // numTripStops (all locations on this journey) / num locations on this journey
                                           * locations.Count   // * Count set of locations for this journey 
                                           * ( 1.0 / tripsTotal );  // 1 / tripTotal (all locaitons on this journey) 
            }

            // Sort the journeys 
            // Now that all Journeys have a desirability index
            // Start with original list 
            List<Journey> newOrdJourneys = new List<Journey>(originalList);
            // Sort the journeys by desirability in descending order 
            newOrdJourneys.Sort( (x, y) => y.DesirabilityValue.CompareTo(x.DesirabilityValue) );

            return newOrdJourneys; 
        }
    }






    // Desirability Algorithm - Reduce CO2 emmissions to minimum 
    // Prioritises shorter routes with less traffic, speeds under 80km,
    // lower emissions transport modes and other causes of increased emissions 
    public class JourneyAlgorithmCO2Min : IJourneyAlgorithm
    {
        public List<Journey> GetJourneysDesirability(List<Journey> originalList)
        {
            List<Journey> newOrdJourneys = originalList;

            // TODO - Implementation 
            // Temporarily just assign original list to return 

            return newOrdJourneys;
        }
    }


    #endregion 

}
