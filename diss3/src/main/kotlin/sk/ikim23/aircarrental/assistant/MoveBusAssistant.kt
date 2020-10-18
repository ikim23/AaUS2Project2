package sk.ikim23.aircarrental.assistant

import OSPABA.*
import sk.ikim23.aircarrental.*
import sk.ikim23.aircarrental.agent.BusAgent
import sk.ikim23.aircarrental.entity.Place
import sk.ikim23.aircarrental.message.MyMessage
import sk.ikim23.aircarrental.sim.Config
import sk.ikim23.aircarrental.sim.ID
import sk.ikim23.aircarrental.sim.MessageNotImplementedException
import sk.ikim23.aircarrental.sim.withCast
import java.util.*

class MoveBusAssistant(mySim: Simulation, val myAgent: BusAgent) : ContinualAssistant(ID.ASS_MOVE_BUS, mySim, myAgent) {
    override fun processMessage(messageForm: MessageForm) {
        withCast(this, messageForm) { msg: MyMessage ->
            when (msg.code()) {
                M.INIT -> {
                    initBuses()
                }
                M.MOVE_BUS -> {
                    val bus = msg.bus
                    if (bus != null) {
                        msg.setCode(M.MOVE_BUS_DONE)
                        when (bus.destination) {
                            Place.T1 -> {
                                bus.nextDestination(Place.T2, mySim().currentTime() + Config.tT1ToT2)
                                hold(Config.tT1ToT2, msg)
                            }
                            Place.T2 -> {
                                bus.nextDestination(Place.SERVICE_DESK, mySim().currentTime() + Config.tT2ToAcr)
                                hold(Config.tT2ToAcr, msg)
                            }
                            Place.T3 -> {
                                bus.nextDestination(Place.T1, mySim().currentTime() + Config.tT3ToT1)
                                hold(Config.tT3ToT1, msg)
                            }
                            Place.SERVICE_DESK -> {
                                if (bus.isEmpty()) {
                                    bus.nextDestination(Place.T1, mySim().currentTime() + Config.tAcrToT1)
                                    hold(Config.tAcrToT1, msg)
                                } else {
                                    bus.nextDestination(Place.T3, mySim().currentTime() + Config.tAcrToT3)
                                    hold(Config.tAcrToT3, msg)
                                }
                            }
                        }
                    }
                }
                M.MOVE_BUS_DONE -> {
                    notice(msg)
                }
                else -> throw MessageNotImplementedException(msg, this)
            }
        }
    }

    private fun initBuses() {
        val rand = Random()
        val destinations = Place.values()
        myAgent.buses.forEach { bus ->
            val arrivalTime = mySim().currentTime() + rand.nextDouble() * 10
            bus.nextDestination(destinations[rand.nextInt(destinations.size)], arrivalTime)
            val mBusArrived = MyMessage(mySim(), M.MOVE_BUS_DONE, bus = bus)
            hold(arrivalTime, mBusArrived)
        }
    }
}