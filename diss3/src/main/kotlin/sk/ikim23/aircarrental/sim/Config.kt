package sk.ikim23.aircarrental.sim

import OSPRNG.TriangularRNG
import OSPRNG.UniformContinuousRNG
import sk.ikim23.aircarrental.random.*

class Config {
    companion object {
        private val timeInterval = 15.0 * 60.0
        val warmTime = 4.0 * 3600.0
        val stopArrivalsAtTime = warmTime + timeInterval * 18
        val driverSalary = 12.5
        val employeeSalary = 11.5

        lateinit var passengerSize: NumberRNG
        lateinit var t1Arrival: Rand
        lateinit var t2Arrival: Rand
        lateinit var serviceDeskArrival: Rand

        val tAcrToT1 = kmToSec(2.5)
        val tAcrToT3 = kmToSec(2.9)
        val tT3ToT1 = kmToSec(0.9)
        val tT1ToT2 = kmToSec(0.5)
        val tT2ToAcr = kmToSec(3.4)

        lateinit var loadTime: UniformContinuousRNG
        lateinit var takeOffTime: UniformContinuousRNG

        lateinit var serviceTimeIn: CompoundRNG
        lateinit var serviceTimeOut: CompoundRNG

        const val kmPerHour = 35

        fun kmToSec(distance: Double): Double {
            return 3600.0 / kmPerHour * distance
        }

        fun secToKm(sec: Double): Double {
            return kmPerHour * 3600.0 / sec
        }

        init {
            reset()
        }

        fun reset() {
            passengerSize = NumberRNG(listOf(
                    SplitFactor(.05, 4),
                    SplitFactor(.2, 3),
                    SplitFactor(.4, 2),
                    SplitFactor(1.0, 1)
            ))
            t1Arrival = Rand(listOf(
                    Piece(warmTime + timeInterval, 3600.0 / 4.0),
                    Piece(warmTime + timeInterval * 2, 3600.0 / 8.0),
                    Piece(warmTime + timeInterval * 3, 3600.0 / 12.0),
                    Piece(warmTime + timeInterval * 4, 3600.0 / 15.0),
                    Piece(warmTime + timeInterval * 5, 3600.0 / 18.0),
                    Piece(warmTime + timeInterval * 6, 3600.0 / 14.0),
                    Piece(warmTime + timeInterval * 7, 3600.0 / 13.0),
                    Piece(warmTime + timeInterval * 8, 3600.0 / 10.0),
                    Piece(warmTime + timeInterval * 9, 3600.0 / 4.0),
                    Piece(warmTime + timeInterval * 10, 3600.0 / 6.0),
                    Piece(warmTime + timeInterval * 11, 3600.0 / 10.0),
                    Piece(warmTime + timeInterval * 12, 3600.0 / 14.0),
                    Piece(warmTime + timeInterval * 13, 3600.0 / 16.0),
                    Piece(warmTime + timeInterval * 14, 3600.0 / 15.0),
                    Piece(warmTime + timeInterval * 15, 3600.0 / 7.0),
                    Piece(warmTime + timeInterval * 16, 3600.0 / 3.0),
                    Piece(warmTime + timeInterval * 17, 3600.0 / 4.0),
                    Piece(warmTime + timeInterval * 18, 3600.0 / 2.0)
            ))
            t2Arrival = Rand(listOf(
                    Piece(warmTime + timeInterval, 3600.0 / 3.0),
                    Piece(warmTime + timeInterval * 2, 3600.0 / 6.0),
                    Piece(warmTime + timeInterval * 3, 3600.0 / 9.0),
                    Piece(warmTime + timeInterval * 4, 3600.0 / 15.0),
                    Piece(warmTime + timeInterval * 5, 3600.0 / 17.0),
                    Piece(warmTime + timeInterval * 6, 3600.0 / 19.0),
                    Piece(warmTime + timeInterval * 7, 3600.0 / 14.0),
                    Piece(warmTime + timeInterval * 8, 3600.0 / 6.0),
                    Piece(warmTime + timeInterval * 9, 3600.0 / 3.0),
                    Piece(warmTime + timeInterval * 10, 3600.0 / 4.0),
                    Piece(warmTime + timeInterval * 11, 3600.0 / 21.0),
                    Piece(warmTime + timeInterval * 12, 3600.0 / 14.0),
                    Piece(warmTime + timeInterval * 13, 3600.0 / 19.0),
                    Piece(warmTime + timeInterval * 14, 3600.0 / 12.0),
                    Piece(warmTime + timeInterval * 15, 3600.0 / 5.0),
                    Piece(warmTime + timeInterval * 16, 3600.0 / 2.0),
                    Piece(warmTime + timeInterval * 17, 3600.0 / 3.0),
                    Piece(warmTime + timeInterval * 18, 3600.0 / 3.0)
            ))
            serviceDeskArrival = Rand(listOf(
                    Piece(warmTime + timeInterval, 3600.0 / 12.0),
                    Piece(warmTime + timeInterval * 2, 3600.0 / 9.0),
                    Piece(warmTime + timeInterval * 3, 3600.0 / 18.0),
                    Piece(warmTime + timeInterval * 4, 3600.0 / 28.0),
                    Piece(warmTime + timeInterval * 5, 3600.0 / 23.0),
                    Piece(warmTime + timeInterval * 6, 3600.0 / 21.0),
                    Piece(warmTime + timeInterval * 7, 3600.0 / 16.0),
                    Piece(warmTime + timeInterval * 8, 3600.0 / 11.0),
                    Piece(warmTime + timeInterval * 9, 3600.0 / 17.0),
                    Piece(warmTime + timeInterval * 10, 3600.0 / 22.0),
                    Piece(warmTime + timeInterval * 11, 3600.0 / 36.0),
                    Piece(warmTime + timeInterval * 12, 3600.0 / 24.0),
                    Piece(warmTime + timeInterval * 13, 3600.0 / 32.0),
                    Piece(warmTime + timeInterval * 14, 3600.0 / 16.0),
                    Piece(warmTime + timeInterval * 15, 3600.0 / 13.0),
                    Piece(warmTime + timeInterval * 16, 3600.0 / 13.0),
                    Piece(warmTime + timeInterval * 17, 3600.0 / 5.0),
                    Piece(warmTime + timeInterval * 18, 3600.0 / 4.0)
            ))
            loadTime = UniformContinuousRNG(10.0, 14.0)
            takeOffTime = UniformContinuousRNG(2.0, 10.0)
            serviceTimeIn = CompoundRNG(
                    0.765625,
                    TriangularRNG(1.46 * 60, 1.98 * 60, 3.0 * 60),
                    TriangularRNG(3.0 * 60, 4.68 * 60, 5.3 * 60)
            )
            serviceTimeOut = CompoundRNG(
                    0.86605317,
                    TriangularRNG(0.999 * 60, 1.31 * 60, 2.21 * 60),
                    TriangularRNG(2.71 * 60, 4.3 * 60, 4.99 * 60)
            )
        }
    }
}