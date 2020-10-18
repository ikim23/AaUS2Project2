package sk.ikim23.aircarrental.entity

enum class BusType(val capacity: Int, val price: Double) {
    SMALL(12, .28),
    MEDIUM(18, .43),
    LARGE(30, .54)
}