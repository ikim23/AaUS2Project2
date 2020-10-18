package sk.ikim23.aircarrental.stat

class BusStats(
        val id: Int,
        val position: String,
        val capacity: Int,
        val nPassengers: Int,
        val nRounds: Int,
        val avgUsage: Double,
        val lowUsage: Double,
        val uppUsage: Double
)