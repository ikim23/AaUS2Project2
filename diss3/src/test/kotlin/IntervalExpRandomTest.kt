import OSPRNG.NormalRNG
import OSPRNG.TriangularRNG
import OSPRNG.UniformContinuousRNG
import org.junit.Test
import sk.ikim23.aircarrental.random.*
import java.io.FileWriter

class IntervalExpRandomTest {
    val sampleSize = 10_000

    @Test
    fun arrivingPassengerServiceTimeTest() {
        val rand = CompoundRNG(
                0.765625,
                NormalRNG(2.18, 0.332),
                TriangularRNG(3.1, 4.54, 5.1)
        )
        FileWriter("arrivingPassengerServiceTimeTest.txt").use {
            for (i in 1..sampleSize) {
                val sample = rand.sample()
                it.write("$sample\n")
            }
        }
    }

    @Test
    fun expTest() {
        val rand = Rand(listOf())
        var sum = 0.0
        for (i in 1..sampleSize) {
            val sample = rand.exponential(12.0)
            println(sample)
            sum += sample
        }
        println("avg: ${(sum / sampleSize)}")
    }

    @Test
    fun piecewiseAllTest() {
        piecewiseTest(listOf(
                Piece(900, 3600.0 / 4.0),
                Piece(900 * 2, 3600.0 / 8.0),
                Piece(900 * 3, 3600.0 / 12.0),
                Piece(900 * 4, 3600.0 / 15.0),
                Piece(900 * 5, 3600.0 / 18.0),
                Piece(900 * 6, 3600.0 / 14.0),
                Piece(900 * 7, 3600.0 / 13.0),
                Piece(900 * 8, 3600.0 / 10.0),
                Piece(900 * 9, 3600.0 / 4.0),
                Piece(900 * 10, 3600.0 / 6.0),
                Piece(900 * 11, 3600.0 / 10.0),
                Piece(900 * 12, 3600.0 / 14.0),
                Piece(900 * 13, 3600.0 / 16.0),
                Piece(900 * 14, 3600.0 / 15.0),
                Piece(900 * 15, 3600.0 / 7.0),
                Piece(900 * 16, 3600.0 / 3.0),
                Piece(900 * 17, 3600.0 / 4.0),
                Piece(900 * 18, 3600.0 / 2.0)
        ), "piecewise-t1.txt")
        piecewiseTest(listOf(
                Piece(900, 3600.0 / 3.0),
                Piece(900 * 2, 3600.0 / 6.0),
                Piece(900 * 3, 3600.0 / 9.0),
                Piece(900 * 4, 3600.0 / 15.0),
                Piece(900 * 5, 3600.0 / 17.0),
                Piece(900 * 6, 3600.0 / 19.0),
                Piece(900 * 7, 3600.0 / 14.0),
                Piece(900 * 8, 3600.0 / 6.0),
                Piece(900 * 9, 3600.0 / 3.0),
                Piece(900 * 10, 3600.0 / 4.0),
                Piece(900 * 11, 3600.0 / 21.0),
                Piece(900 * 12, 3600.0 / 14.0),
                Piece(900 * 13, 3600.0 / 19.0),
                Piece(900 * 14, 3600.0 / 12.0),
                Piece(900 * 15, 3600.0 / 5.0),
                Piece(900 * 16, 3600.0 / 2.0),
                Piece(900 * 17, 3600.0 / 3.0),
                Piece(900 * 18, 3600.0 / 3.0)
        ), "piecewise-t2.txt")
        piecewiseTest(listOf(
                Piece(900, 3600.0 / 12.0),
                Piece(900 * 2, 3600.0 / 9.0),
                Piece(900 * 3, 3600.0 / 18.0),
                Piece(900 * 4, 3600.0 / 28.0),
                Piece(900 * 5, 3600.0 / 23.0),
                Piece(900 * 6, 3600.0 / 21.0),
                Piece(900 * 7, 3600.0 / 16.0),
                Piece(900 * 8, 3600.0 / 11.0),
                Piece(900 * 9, 3600.0 / 17.0),
                Piece(900 * 10, 3600.0 / 22.0),
                Piece(900 * 11, 3600.0 / 36.0),
                Piece(900 * 12, 3600.0 / 24.0),
                Piece(900 * 13, 3600.0 / 32.0),
                Piece(900 * 14, 3600.0 / 16.0),
                Piece(900 * 15, 3600.0 / 13.0),
                Piece(900 * 16, 3600.0 / 13.0),
                Piece(900 * 17, 3600.0 / 5.0),
                Piece(900 * 18, 3600.0 / 4.0)
        ), "piecewise-sd.txt")
    }

    fun piecewiseTest(pieces: List<Piece>, fileName: String) {
        val rand = Rand(pieces)
        val reps = 10_000
        val maxTime = pieces.last().endTime
        var currentTime = .0
        FileWriter(fileName).use {
            for (rep in 1..reps) {
                val sample = rand.sample(currentTime)
                currentTime += sample
                if (currentTime >= maxTime) {
                    currentTime = .0
                }
                it.write("$currentTime\n")
            }
        }
    }

    @Test
    fun numberRngTest() {
        val rand = NumberRNG(listOf(
                SplitFactor(.05, 4),
                SplitFactor(.2, 3),
                SplitFactor(.4, 2),
                SplitFactor(1.0, 1)
        ))
        val reps = 10_000
        val buckets = mutableMapOf<Int, Int>()
        for (rep in 1..reps) {
            val key = rand.sample()
            var count = buckets[key]
            if (count == null) {
                buckets[key] = 1
            } else {
                buckets[key] = count + 1
            }
        }
        for ((key, count) in buckets.toSortedMap()) {
            println("$key: ${count.toDouble() / reps}")
        }
    }

    @Test
    fun datTest() {
        val serviceTimeIn = CompoundRNG(
                0.765625,
                TriangularRNG(1.46, 1.99, 3.0),
                TriangularRNG(3.0, 4.36, 5.3)
        )
        compoundTest(serviceTimeIn, "inDat.txt")
        val serviceTimeOut = CompoundRNG(
                0.86605317,
                TriangularRNG(0.999, 1.33, 2.21),
                TriangularRNG(2.71, 4.3, 4.99)
        )
        compoundTest(serviceTimeOut, "outDat.txt")
    }

    fun compoundTest(rand: CompoundRNG, fileName: String) {
        val reps = 10_000
        FileWriter(fileName).use {
            for (rep in 1..reps) {
                val sample = rand.sample()
                it.write("$sample\n")
            }
        }
    }

    @Test
    fun uniformTest() {
        val loadTime = UniformContinuousRNG(10.0, 14.0)
        val reps = 10_000
        var sum = .0
        for (i in 1..reps) {
            sum += loadTime.sample()
        }
        println("avg: ${sum / reps}")
    }
}