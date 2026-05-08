
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  Tooltip,
  ResponsiveContainer,
  CartesianGrid
} from "recharts";

import { Card } from "@/components/ui/card";

export default function RevenueChart({ data }) {

  return (

    <Card className="p-6 rounded-2xl">

      <div className="mb-5">

        <h2 className="text-xl font-semibold">
          Revenue Overview
        </h2>

        <p className="text-gray-500 text-sm">
          Weekly revenue performance
        </p>

      </div>

      <ResponsiveContainer width="100%" height={350}>

        <LineChart data={data}>

          <CartesianGrid strokeDasharray="3 3" />

          <XAxis dataKey="label" />

          <YAxis />

          <Tooltip />

          <Line
            type="monotone"
            dataKey="revenue"
            strokeWidth={3}
            dot={false}
          />

        </LineChart>

      </ResponsiveContainer>

    </Card>

  );

}