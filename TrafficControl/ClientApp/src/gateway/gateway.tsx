import axios from "axios";

const beBaseUri = "http://localhost:5052/api/";

export class BEGateway {
    async getScenarios() {
        const response = await axios.get(beBaseUri + 'scenarios');
        return response.data;
    }

    async createScenario(name: any, networkFile: any, routeFile: any) {
        const formData = new FormData();

        formData.append('name', name);
        formData.append('networkFile', networkFile, networkFile.name);
        formData.append('routeFile', routeFile, routeFile.name);

        await axios.post(beBaseUri + 'scenarios', formData);
    }

    async getSimulations() {
        const response = await axios.get(beBaseUri + 'simulations');
        return response.data;
    }

    async getSimulation(id: number) {
        const response = await axios.get(beBaseUri + 'simulations/' + id);
        return response.data;
    }

    async startSimulation(scenarioId: number, name: string) {
        await axios.post(beBaseUri + 'simulations', { scenarioId, name })
    }
}

export const Gateway = new BEGateway();