package sk.ikim23.carrental.core.obj

class Customer(val arrivalTime: Double) {
    companion object {
        var ID = 1
    }

    val id = ID++
    var serviceTime = 0.0
    var serviceDeskNumber = 0

    override fun toString() = "Customer(id=$id)"
}