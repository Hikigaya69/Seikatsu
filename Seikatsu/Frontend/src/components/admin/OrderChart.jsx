import {
    LineChart,
    Line,
    XAxis,
    YAxis,
    Tooltip,
    ResponsiveContainer,
    CartesianGrid,
} from "recharts";

export default function OrdersChart({ data }) {
    return (
        <div className="bg-white rounded-2xl p-6 border shadow-sm">
            <h2 className="text-xl font-semibold mb-5">Orders Overview</h2>
            <div className="h-[350px]">
                <ResponsiveContainer width="100%" height="100%">
                    <LineChart data={data}>
                        <CartesianGrid strokeDasharray="3 3" />
                        <XAxis dataKey="label" />
                        <YAxis />
                        <Tooltip />
                        <Line
                            type="monotone"
                            dataKey="confirmedOrders"
                            stroke="#2563eb"
                            strokeWidth={3}
                        />
                        <Line
                            type="monotone"
                            dataKey="pendingOrders"
                            stroke="#f59e0b"
                            strokeWidth={3}
                        />
                    </LineChart>
                </ResponsiveContainer>
            </div>
        </div>
    );
}