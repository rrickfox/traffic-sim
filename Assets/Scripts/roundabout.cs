using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DataTypes;
using static Pathfinding.Pathfinding;

//erzeugt eine spezielle T-kreuzug ("protoRoundabout"), aus der Kreisverkehre gebaut werden können. Da ein Kreisverkehr nur in eine Richtung befahrbar ist, bedeutet das für die T-kreuzung, dass eine Straße beliebeige Gestalt haben darf, die anderen beiden jedoch Einbahnstraßen sein müssen (eine von der Kreuzung weg, die andere zur Kreuzung hin)  

public class protoRoundabout : Vertex<TeeSection, TeeSectionBehaviour>
{
    public GameObject roadPrefab;
    public GameObject carPrefab;
    private Edge _incoming { get; }
    private Edge _outgoing { get; }
    private Edge _Right { get; }

        
    public protoRoundabout(GameObject prefab, Edge incoming, Edge outgoing, Edge Right)
        : base(prefab, incoming, outgoing, Right)
    {
        _incoming = incoming;
        _outgoing = outgoing;
        _Right = Right;
    }

    // returns necessary lane to go from an edge to another edge
    // throws exception if edges are not in this vertex
    // throws exception if edges are equal
    public override LaneType SubRoute(Edge comingFrom, Edge to)
    {
        var from = comingFrom.other; // Subroute gets called with the Edge facing this Vertex, therefore other must be called
        if (!edges.Contains(from)) throw new NetworkConfigurationError("From Edge not found");
        if(!edges.Contains(to)) throw new NetworkConfigurationError("To Edge not found");
        if(from == to) throw new NetworkConfigurationError("From and to are the same Edge");
        
        if(from == _Right)
            if(to == _incoming)
                throw new NetworkConfigurationError("to is incoming-only")
            else // to == _outgoing
                return LaneType.RightTurn;
        if(from == _incoming)
            if(to == _Right)
                return LaneType.RightTurn;
            else // to == _outgoing
                return LaneType.Through;
        else // from == _outgoing
            throw new NetworkConfigurationError("coming from outgoing-only")
        }
    }
}