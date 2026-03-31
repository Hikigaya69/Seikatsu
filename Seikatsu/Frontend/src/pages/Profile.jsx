import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

import StoreLayout from "../layouts/StoreLayout";
import { ArrowLeft } from "lucide-react";
import api from "../Utils/api";

export default function Profile() {

    const [activeTab, setActiveTab] = useState("overview");
    const [profile, setProfile] = useState(null);
    const [ordersOverview, setOrdersOverview] = useState(null);
    const navigate = useNavigate();

    useEffect(() => {

        const loadProfile = async () => {

            const profileRes = await api.get(
                "UserProfile/getprofile",
                { withCredentials: true }
            );

            const overviewRes = await api.get(
                "UserProfile/getorderview", { withCredentials: true }
                
            );

            setProfile(profileRes.data.data);
            setOrdersOverview(overviewRes.data.data);

        };

        loadProfile();

    }, []);

    if (!profile || !ordersOverview)
        return <StoreLayout>Loading...</StoreLayout>;

    return (

        <StoreLayout>

            <div className="max-w-7xl mx-auto px-6 py-10">

                <h1 className="text-3xl font-bold mb-6">
                    My Account
                </h1>
                <button
                    onClick={() => navigate("/home")}
                    className="flex items-center gap-2 border border-gray-300 px-4 py-2 rounded-lg shadow-sm hover:shadow-md hover:bg-gray-50 transition mb-6"
                >
                    <ArrowLeft size={18} />
                    Go back to Home
                </button>

                <div className="grid grid-cols-4 gap-8">

                    {/* LEFT SIDEBAR */}
                    <div className="bg-white shadow rounded-xl p-6 text-center">

                        <img
                            src="https://i.pravatar.cc/150"
                            className="w-24 h-24 rounded-full mx-auto mb-4"
                        />

                        <h2 className="font-semibold text-lg">
                            {profile.name}
                        </h2>

                        <p className="text-gray-500 text-sm">
                            {profile.email}
                        </p>

                    </div>


                    {/* RIGHT PANEL */}
                    <div className="col-span-3">

                        <Tabs
                            activeTab={activeTab}
                            setActiveTab={setActiveTab}
                        />

                        {activeTab === "overview" &&
                            <OverviewTab data={ordersOverview} />}

                        {activeTab === "orders" &&
                            <OrdersTab />}

                        {activeTab === "addresses" &&
                            <AddressesTab />}

                        {activeTab === "personal" &&
                            <PersonalInfoTab profile={profile} />}

                    </div>

                </div>

            </div>

        </StoreLayout>

    );
}

function Tabs({ activeTab, setActiveTab }) {

    const tabs = ["overview", "orders", "addresses", "personal"];

    return (

        <div className="flex gap-4 mb-6">

            {tabs.map(tab => (

                <button
                    key={tab}
                    onClick={() => setActiveTab(tab)}
                    className={`px-6 py-2 rounded-full border
          ${activeTab === tab
                            ? "bg-[#3c6e71] text-white"
                            : "bg-white"}
          `}
                >
                    {tab}

                </button>

            ))}

        </div>

    );

}

function OverviewTab({ data }) {

    return (

        <div className="grid grid-cols-4 gap-6">

            <Stat title="Total Orders" value={data.totalOrders} />
            <Stat title="Total Spent" value={`¥${data.totalAmountSpent}`} />
            <Stat title="Pending" value={data.pendingOrders} />
            <Stat title="Delivered" value={data.deliveredOrders} />

        </div>

    );

}

function Stat({ title, value }) {

    return (

        <div className="bg-white shadow rounded-xl p-6">

            <p className="text-gray-500">
                {title}
            </p>

            <p className="text-2xl font-bold">
                {value}
            </p>

        </div>

    );

}



function OrdersTab() {

    const [orders, setOrders] = useState([]);
    const [year, setYear] = useState("");

    const fetchOrders = async (selectedYear = "") => {

        try {

            const url = selectedYear
                ? `Order/orderhistory/${selectedYear}`
                : "Order/orderhistory";

            const res = await api.get(url, {
                withCredentials: true
            });

            setOrders(res.data.data);

        } catch (err) {

            console.error(err);

        }

    };

    useEffect(() => {

        fetchOrders();

    }, []);

    return (

        <div>

            {/* YEAR FILTER */}
            <div className="mb-6">

                <select
                    value={year}
                    onChange={(e) => {

                        setYear(e.target.value);
                        fetchOrders(e.target.value);

                    }}
                    className="border px-4 py-2 rounded-lg"
                >

                    <option value="">Last 5 Months</option>
                    <option value="2026">2026</option>
                    <option value="2025">2025</option>
                    <option value="2024">2024</option>

                </select>

            </div>


            {/* ORDER LIST */}
            <div className="space-y-4">

                {orders.map(order => (

                    <OrderCard key={order.orderItemId} order={order} />

                ))}

            </div>

        </div>

    );

}


function OrderCard({ order }) {

    const navigate = useNavigate();

    return (

        <div className="bg-white shadow rounded-xl p-4 flex justify-between">

            <div className="flex gap-4">

                <img
                    src={order.productImageUrl}
                    className="w-16 h-16 rounded"
                />

                <div>

                    <p className="font-semibold">
                        {order.productName}
                    </p>

                    <p className="text-gray-500 text-sm">
                        Qty: {order.quantity}
                    </p>

                    <p className="text-gray-400 text-sm">
                        {new Date(order.orderedDate)
                            .toLocaleDateString()}
                    </p>

                </div>

            </div>


            {/* RIGHT SIDE */}
            <div className="flex flex-col items-end gap-2">

                <span className="text-green-600 font-semibold">
                    {order.orderStatus}
                </span>

                <button
                    onClick={() =>
                        navigate(`/order-summary/${order.orderId}`)
                    }
                    className="border px-4 py-1 rounded-lg hover:bg-gray-100"
                >

                    View Details

                </button>

            </div>

        </div>

    );

}

function AddressesTab() {

    const [addresses, setAddresses] = useState([]);

    useEffect(() => {

        api.get(
            "Address/getalladdress",
            { withCredentials: true }
        )
            .then(res => setAddresses(res.data.data));

    }, []);

    return (

        <div className="space-y-4">

            {addresses.map(addr => (

                <div
                    key={addr.id}
                    className="bg-white shadow rounded-xl p-4"
                >

                    <p>{addr.fullName}</p>
                    <p>{addr.addressLine1}</p>
                    <p>{addr.city}</p>
                    <p>{addr.postalCode}</p>

                    {addr.isDefault &&
                        <span className="text-green-600 text-sm">
                            Default Address
                        </span>}

                </div>

            ))}

        </div>

    );

}

function PersonalInfoTab({ profile }) {

    const [form, setForm] = useState(profile);

    const updateProfile = async () => {

        await api.post(
            "UserProfile/updateprofile",
            form,
            { withCredentials: true }
        );

        alert("Profile updated successfully");

    };

    return (

        <div className="bg-white shadow rounded-xl p-6 space-y-4">

            <input
                value={form.name}
                onChange={e =>
                    setForm({ ...form, name: e.target.value })}
                className="border p-2 w-full"
            />

            <input
                value={form.email}
                onChange={e =>
                    setForm({ ...form, email: e.target.value })}
                className="border p-2 w-full"
            />

            <button
                onClick={updateProfile}
                className="bg-[#3c6e71] text-white px-4 py-2 rounded"
            >

                Save Changes

            </button>

        </div>

    );

}