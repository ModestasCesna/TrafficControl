import { useEffect, useState } from "react";
import {Link} from "react-router-dom";
import { Gateway } from "../gateway/gateway";

export const Scenarios = () => {
    const [scenarios, setScenarios] = useState<any>([]);

    useEffect(() => {
        fetchData()
    }, []);

    const fetchData = async () => {
        const data = await Gateway.getScenarios();
        setScenarios(data);
    }

    return <>
        <h1 className="mb-4">Scenarijai</h1>
        <div className="row d-flex">
            <div className="col-4 mb-3">
                <Link to="/scenarios/add">
                    <div className="card" style={{minHeight: 88}}>
                        <div className="card-body d-flex align-items-center">
                            <h5 className="card-title">
                                Pridėti naują scenarijų
                            </h5>
                        </div>
                    </div>
                </Link>
            </div>

            {scenarios.map((s: any) => 
            <div className="col-4 mb-3" key={s.id}>
                <div className="card" style={{minHeight: 88}}>
                    <div className="card-body">
                        <h5 className="card-title">
                            {s.name}
                        </h5>
                        <h6 className="card-subtitle mb-2 text-muted">
                            {new Date(s.created).toLocaleString()}
                        </h6>
                    </div>
                </div>
            </div>)}
        </div>
    </>
}