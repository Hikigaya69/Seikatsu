import { useEffect, useState } from "react";

import api from "../../Utils/api";

import StatsCard from "../../components/admin/StatsCard";

export default function Dashboard() {

  const [customers,setCustomers] = useState(0);

  const [revenue,setRevenue] = useState(0);

  const [orders,setOrders] = useState({
    totalOrders:0
  });

  const [categories,setCategories] = useState([]);

  useEffect(()=>{

    fetchDashboard();

  },[]);


  const fetchDashboard = async()=>{

    try{

      const [
        customerRes,
        revenueRes,
        ordersRes,
        categoryRes
      ] = await Promise.all([

        api.get("/Admin/customerscount"),

        api.get("/Admin/totalrevenue"),

        api.get("/Admin/totalorders"),

        api.get("/Admin/categoryitemcount")

      ]);

      setCustomers(customerRes.data.data);

      setRevenue(revenueRes.data.data);

      setOrders(ordersRes.data.data);

      setCategories(categoryRes.data.data);

    }catch(err){

      console.error(err);

    }

  };


  return (

    <div className="space-y-8">

      <div>

        <h1 className="text-4xl font-bold">
          Dashboard
        </h1>

        <p className="text-gray-500 mt-2">
          Monitor store performance
        </p>

      </div>


      <div className="grid grid-cols-4 gap-6">

        <StatsCard
          title="Revenue"
          value={`¥${revenue}`}
          subtitle="Total revenue"
        />

        <StatsCard
          title="Orders"
          value={orders.totalOrders}
          subtitle="Total orders"
        />

        <StatsCard
          title="Customers"
          value={customers}
          subtitle="Registered customers"
        />

        <StatsCard
          title="Confirmed Orders"
          value={orders.confirmedOrders}
          subtitle="Successful orders"
        />

      </div>


      <div className="grid grid-cols-3 gap-6">

        <div className="col-span-2 bg-white rounded-2xl p-6 border shadow-sm">

          <h2 className="text-xl font-semibold mb-5">
            Order Summary
          </h2>

          <div className="grid grid-cols-3 gap-4">

            <div className="bg-[#f8fafc] rounded-xl p-5">

              <p className="text-gray-500">
                Pending
              </p>

              <h2 className="text-3xl font-bold mt-2">
                {orders.pendingOrders}
              </h2>

            </div>


            <div className="bg-[#f8fafc] rounded-xl p-5">

              <p className="text-gray-500">
                Created
              </p>

              <h2 className="text-3xl font-bold mt-2">
                {orders.createdOrders}
              </h2>

            </div>


            <div className="bg-[#f8fafc] rounded-xl p-5">

              <p className="text-gray-500">
                Confirmed
              </p>

              <h2 className="text-3xl font-bold mt-2">
                {orders.confirmedOrders}
              </h2>

            </div>

          </div>

        </div>


        <div className="bg-white rounded-2xl p-6 border shadow-sm">

          <h2 className="text-xl font-semibold mb-5">
            Categories
          </h2>

          <div className="space-y-4">

            {categories.map(category=>(

              <div
                key={category.id}
                className="flex justify-between items-center bg-[#f8fafc] p-4 rounded-xl"
              >

                <span>
                  {category.categoryName}
                </span>

                <span className="font-semibold">
                  {category.productCount}
                </span>

              </div>

            ))}

          </div>

        </div>

      </div>

    </div>

  );

}