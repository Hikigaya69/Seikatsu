import { useEffect, useState } from "react";
import RevenueChart from "../../components/admin/RevenueChart";
import OrderChart from "../../components/admin/OrderChart";

import api from "../../Utils/api";

import {
  TrendingUp,
  ShoppingBag,
  CircleDollarSign
} from "lucide-react";

export default function Analytics() {

  const [revenue,setRevenue] = useState(null);

  const [orders,setOrders] = useState(null);

  const [period,setPeriod] = useState(2);

  useEffect(()=>{

    fetchAnalytics();

  },[period]);


  const fetchAnalytics = async()=>{

    try{

      const requestBody = {
        period
      };

      const [
        revenueRes,
        orderRes
      ] = await Promise.all([

        api.post(
          "/Admin/revenueforperiod",
          
            requestBody
          
        ),

        api.post(
          "/Admin/ordersforperiod",
          
            requestBody
          
        )

      ]);

      setRevenue(revenueRes.data.data);

      setOrders(orderRes.data.data);

    }catch(err){

      console.error(err);

    }

  };


  return (

    <div className="space-y-8">

      <div className="flex items-center justify-between">

        <div>

          <h1 className="text-4xl font-bold">
            Analytics
          </h1>

          <p className="text-gray-500 mt-2">
            Revenue and order insights
          </p>

        </div>


        <select
          value={period}
          onChange={(e)=>
            setPeriod(Number(e.target.value))
          }
          className="border rounded-xl px-4 py-3"
        >

          <option value={0}>
            Today
          </option>

          <option value={1}>
            Weekly
          </option>

          <option value={2}>
            Monthly
          </option>

          <option value={3}>
            Yearly
          </option>

        </select>

      </div>


      <div className="grid grid-cols-3 gap-6">

        <div className="bg-white rounded-2xl p-6 border shadow-sm">

          <div className="flex items-center justify-between">

            <div>

              <p className="text-gray-500">
                Total Revenue
              </p>

              <h2 className="text-3xl font-bold mt-3">

                ₹{revenue?.totalRevenue || 0}

              </h2>

            </div>

            <CircleDollarSign size={34}/>

          </div>

        </div>


        <div className="bg-white rounded-2xl p-6 border shadow-sm">

          <div className="flex items-center justify-between">

            <div>

              <p className="text-gray-500">
                Total Orders
              </p>

              <h2 className="text-3xl font-bold mt-3">

                {orders?.totalOrders || 0}

              </h2>

            </div>

            <ShoppingBag size={34}/>

          </div>

        </div>


        <div className="bg-white rounded-2xl p-6 border shadow-sm">

          <div className="flex items-center justify-between">

            <div>

              <p className="text-gray-500">
                Confirmed Orders
              </p>

              <h2 className="text-3xl font-bold mt-3">

                {orders?.confirmedOrders || 0}

              </h2>

            </div>

            <TrendingUp size={34}/>

          </div>

        </div>

      </div>
          <RevenueChart data={revenue?.dataPoints || []} />
          <OrderChart data={orders?.dataPoints || []} />       

      <div className="grid grid-cols-2 gap-6">

        <div className="bg-white rounded-2xl p-6 border shadow-sm">

          <h2 className="text-xl font-semibold mb-5">
            Revenue Breakdown
          </h2>

          <div className="space-y-4">

            {revenue?.dataPoints?.map((point,index)=>(

              <div
                key={index}
                className="flex justify-between items-center bg-[#f8fafc] rounded-xl p-4"
              >

                <span>
                  {point.label}
                </span>

                <span className="font-semibold">
                        ₹{point.revenue}
                </span>

              </div>

            ))}

          </div>

        </div>


        <div className="bg-white rounded-2xl p-6 border shadow-sm">

          <h2 className="text-xl font-semibold mb-5">
            Orders Breakdown
          </h2>

          <div className="space-y-4">

            {orders?.dataPoints?.map((point,index)=>(

              <div
                key={index}
                className="bg-[#f8fafc] rounded-xl p-4"
              >

                <div className="flex justify-between mb-3">

                  <span className="font-medium">
                    {point.label}
                  </span>

                  <span>
                    {point.totalOrders} Orders
                  </span>

                </div>


                <div className="flex gap-3 text-sm">

                  <span className="bg-green-100 text-green-700 px-2 py-1 rounded">

                    Confirmed:
                    {point.confirmedOrders}

                  </span>


                  <span className="bg-yellow-100 text-yellow-700 px-2 py-1 rounded">

                    Pending:
                    {point.pendingOrders}

                  </span>


                  <span className="bg-blue-100 text-blue-700 px-2 py-1 rounded">

                    Created:
                    {point.createdOrders}

                  </span>

                </div>

              </div>

            ))}

          </div>

        </div>

      </div>

    </div>

  );

}