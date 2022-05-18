import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { CartesianGrid, Legend, Line, LineChart, ResponsiveContainer, XAxis, YAxis } from "recharts";
import { Gateway } from "../gateway/gateway";

export const Simulation = () => {
    const params = useParams();
    const [simulation, setSimulation] = useState<any>({});
    const [stoppedChartData, setStoppedChartData] = useState<any>([]);
    const [waitingTimeChartData, setWaitingTimeChartData] = useState<any>([]);
    const [rewardChartData, setRewardChartData] = useState<any>([]);
    const [intersections, setIntersections] = useState<any>([]);

    const colors = {
        0: "#1abc9c",
        1: "#3498db",
        2: "#9b59b6",
        3: "#e67e22",
        4: "#34495e"
    }
    
    useEffect(() => {
        fetchData();
    }, [])

    const fetchData = async () => {
        const data = await Gateway.getSimulation((params as any).id);
        setSimulation(data);
        mapChartData(data.results);
    }

    const mapChartData = (results: any[]) => {
        const sorted = results.sort((a, b) => a.episode - b.episode)
        const stopped: any[] = [];
        const waiting: any[] = [];
        const reward: any[] = [];
        const intersections: any[] = [];
        
        sorted.forEach(element => {
            const intersection = intersections.find(o => o === element.intersectionId)
            if(!intersection) {
                intersections.push(element.intersectionId)
            }

            const found = stopped.find(o => o.episode === element.episode)
            if(!found) {
                stopped.push({episode: element.episode});
                waiting.push({episode: element.episode});
                reward.push({episode: element.episode});
            }

            const stoppedElement = stopped.find(o => o.episode === element.episode)
            stoppedElement[element.intersectionId] = element.stoppedVehiclesAverage

            const waitingElement = waiting.find(o => o.episode === element.episode)
            waitingElement[element.intersectionId] = element.waitingTimeAverage

            const rewardElement = reward.find(o => o.episode === element.episode)
            rewardElement[element.intersectionId] = element.reward
        });

        setStoppedChartData(stopped);
        setWaitingTimeChartData(waiting);
        setRewardChartData(reward);
        setIntersections(intersections);
    }

    return <>
        <h1>{simulation.name}</h1>
        <div className="row">
            <h3>Atlygis</h3>
            <div className="col-12" style={{minHeight: 300}}>
                <ResponsiveContainer width="100%" height="100%">
                    <LineChart
                    width={500}
                    height={300}
                    data={rewardChartData}
                    margin={{
                        top: 5,
                        right: 30,
                        left: 20,
                        bottom: 5,
                    }}
                    >
                        <CartesianGrid strokeDasharray="3 3" />
                        <XAxis dataKey="episode" />
                        <YAxis />
                        <Legend />
                        {intersections.map((i: any, c: number) => <Line type="monotone" key={"reward" + i} dataKey={i} stroke={(colors as any)[c]} />)}
                    </LineChart>
                </ResponsiveContainer>
            </div>
        </div>
        <div className="row">
            <h3>Vidutinis sustojusių automobilių skaičius</h3>
            <div className="col-12" style={{minHeight: 300}}>
                <ResponsiveContainer width="100%" height="100%">
                    <LineChart
                    width={500}
                    height={300}
                    data={stoppedChartData}
                    margin={{
                        top: 5,
                        right: 30,
                        left: 20,
                        bottom: 5,
                    }}
                    >
                        <CartesianGrid strokeDasharray="3 3" />
                        <XAxis dataKey="episode" />
                        <YAxis />
                        <Legend />
                        {intersections.map((i: any, c: number) => <Line type="monotone" key={"stopped" + i} dataKey={i} stroke={(colors as any)[c]} />)}
                    </LineChart>
                </ResponsiveContainer>
            </div>
        </div>
        <div className="row">
            <h3>Vidutinis juostų sukauptas laukimo laikas</h3>
            <div className="col-12" style={{minHeight: 300}}>
                <ResponsiveContainer width="100%" height="100%">
                    <LineChart
                    width={500}
                    height={300}
                    data={waitingTimeChartData}
                    margin={{
                        top: 5,
                        right: 30,
                        left: 20,
                        bottom: 5,
                    }}
                    >
                        <CartesianGrid strokeDasharray="3 3" />
                        <XAxis dataKey="episode" />
                        <YAxis />
                        <Legend />
                        {intersections.map((i: any, c: number) => <Line type="monotone" key={"time" + i} dataKey={i} stroke={(colors as any)[c]} />)}
                    </LineChart>
                </ResponsiveContainer>
            </div>
        </div>
    </>;
}