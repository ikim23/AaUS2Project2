package sk.ikim23.aircarrental.entity

class Passenger(val arrivalTime: Double, val arrivalPlace: Place, val size: Int = 1) {
    val id = ++ID
    var queueTime = .0
    var leaveTime = .0

    fun systemTime() = leaveTime - arrivalTime

    override fun toString() = "Passenger(id=$id,arrivalTime=$arrivalTime,size=$size)"

    companion object {
        var ID = 0
    }
}