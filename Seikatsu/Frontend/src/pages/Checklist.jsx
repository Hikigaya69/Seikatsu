import { useEffect, useState } from "react";
import StoreLayout from "../layouts/StoreLayout";
import api from "../Utils/api";
import {
    Trash2,
    Plus,
    ArrowLeft,
    ShoppingCart,
    ChevronDown
} from "lucide-react";
import { motion } from "framer-motion";
import { useNavigate } from "react-router-dom";
import paperBg from "../assets/book3.png";

export default function Checklist() {

    const navigate = useNavigate();

    const [items, setItems] = useState([]);
    const [newItem, setNewItem] = useState("");
    const [suggestions, setSuggestions] = useState([]);
    const [showSuggestions, setShowSuggestions] = useState(false);
    const [sortMode, setSortMode] = useState("priority");
    const [collapseCompleted, setCollapseCompleted] = useState(true);


    /*
    SORT FUNCTION
    */

    const sortChecklist = (list) => {

        const priorityRank = { high: 1, medium: 2, low: 3 };

        let sorted = [...list];

        if (sortMode === "priority")
            sorted.sort((a, b) => priorityRank[a.priority] - priorityRank[b.priority]);

        if (sortMode === "date")
            sorted.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));

        return [
            ...sorted.filter(i => !i.isChecked),
            ...sorted.filter(i => i.isChecked)
        ];

    };

    const sortedItems = sortChecklist(items);


    /*
    FETCH CHECKLIST
    */

    useEffect(() => { fetchChecklist() }, []);

    const fetchChecklist = async () => {

        try {

            const res = await api.get("/Checklist/getchecklist");

            const enriched = res.data.data.items.map(i => ({
                ...i,
                priority: "medium",
                createdAt: new Date(),
                existsInStore: true
            }));

            setItems(enriched);

        } catch (err) { console.error(err) }

    };


    /*
    FETCH SUGGESTIONS
    */

    const fetchSuggestions = async (query) => {

        if (!query.trim()) {
            setSuggestions([]);
            setShowSuggestions(false);
            return;
        }

        try {

            const res = await api.get(`/Product/suggestion/${query}`);
            setSuggestions(res.data.data);
            setShowSuggestions(true);

        } catch (err) { console.log(err) }

    };


    /*
    ADD ITEM
    */

    const addItem = async (name, exists = true) => {

        if (!name.trim()) return;

        const res = await api.post("/Checklist/additem", {
            productName: name,
            isChecked: false
        });

        const newEntry = {
            ...res.data.data,
            priority: "medium",
            createdAt: new Date(),
            existsInStore: exists
        };

        setItems(prev => [newEntry, ...prev]);

        setNewItem("");
        setSuggestions([]);
        setShowSuggestions(false);

    };


    /*
    REQUEST PRODUCT
    */

    const requestProduct = (name) => {

        alert(`Request sent for "${name}" to admin`);

    };



    const toggleItem = async (item) => {

        const res = await api.patch(
            `/Checklist/modifyitem?itemId=${item.id}`,
            {
                productName: item.productName,
                isChecked: !item.isChecked
            }
        );

        const updated = items.map(i =>
            i.id === item.id
                ? { ...res.data.data, priority: item.priority, existsInStore: item.existsInStore }
                : i
        );

        setItems(updated);

    };


    /*
    DELETE
    */

    const deleteItem = async (id) => {

        await api.delete(`/Checklist/deleteitem/${id}`);

        setItems(prev => prev.filter(i => i.id !== id));

    };


    /*
    CLEAR COMPLETED
    */

    const clearCompleted = () => {

        setItems(prev => prev.filter(i => !i.isChecked));

    };


    /*
    ADD TO CART
    */

    const addToCart = async (item) => {

        if (!item.existsInStore) {

            if (confirm("Product unavailable. Send request to admin?"))
                requestProduct(item.productName);

            return;

        }

        const search = await api.get(`/Product/search/${item.productName}`);

        if (search.data.data.length === 0) {

            if (confirm("Product missing. Send request?"))
                requestProduct(item.productName);

            return;

        }

        await api.post("/Cart/additem", {
            productId: search.data.data[0].id,
            quantity: 1
        });

        alert(`${item.productName} added to cart`);

        toggleItem(item);

    };


    /*
    CHANGE PRIORITY
    */

    const changePriority = (id, value) => {

        const updated = items.map(i =>
            i.id === id ? { ...i, priority: value } : i
        );

        setItems(updated);

    };



    const completedCount = items.filter(i => i.isChecked).length;

    const progressPercent =
        items.length === 0
            ? 0
            : Math.round((completedCount / items.length) * 100);


    return (

        <StoreLayout>

            <div
                className="min-h-screen pt-24 pb-30 relative"
                style={{
                    backgroundImage: `url(${paperBg})`,
                    backgroundSize: "cover",
                    backgroundPosition: "center"
                }}
            >

                <div className="max-w-7xl mx-auto px-6 mb-4">

                    <button
                        onClick={() => navigate(-1)}
                        className="flex items-center gap-2 border px-4 py-2 rounded-lg hover:bg-gray-50"
                    >
                        <ArrowLeft size={18} />Go Back
                    </button>

                </div>


                <div className="flex justify-center">

                    <div className="relative">

                        <div className="absolute inset-0 bg-white/40 rounded-2xl rotate-2 translate-x-4 translate-y-4"></div>
                        <div className="absolute inset-0 bg-white/60 rounded-2xl -rotate-1 translate-x-2 translate-y-2"></div>


                        <motion.div
                            initial={{ opacity: 0, y: 60 }}
                            animate={{ opacity: 1, y: 0 }}
                            className="relative bg-white w-[760px] p-12 rounded-2xl shadow-2xl"
                            style={{ fontFamily: "Patrick Hand" }}
                        >

                            <div className="absolute -top-6 right-10 text-3xl opacity-70">📎</div>


                            <div className="flex justify-between mb-2">

                                <h1 className="text-3xl text-gray-700">
                                    Shopping Checklist
                                </h1>

                                <select
                                    value={sortMode}
                                    onChange={e => setSortMode(e.target.value)}
                                    className="border rounded text-xs px-2"
                                >
                                    <option value="priority">Sort by priority</option>
                                    <option value="date">Sort by date</option>
                                </select>

                            </div>


                            <p className="text-sm text-gray-400 mb-3">
                                {completedCount} of {items.length} items completed
                            </p>


                            <div className="w-full bg-gray-200 rounded-full h-2 mb-6">
                                <div
                                    className="bg-blue-400 h-2 rounded-full"
                                    style={{ width: `${progressPercent}%` }}
                                />
                            </div>


                            {/* INPUT n SUGGESTIONS */}

                            <div className="relative flex gap-3 mb-6">

                                <input
                                    value={newItem}
                                    onChange={e => {
                                        setNewItem(e.target.value);
                                        fetchSuggestions(e.target.value);
                                    }}
                                    placeholder="Write something..."
                                    className="flex-1 border-b bg-transparent focus:outline-none"
                                />

                                <button
                                    onClick={() => addItem(newItem, true)}
                                    className="bg-blue-100 text-blue-600 px-3 py-1 rounded-lg"
                                >
                                    <Plus size={18} />
                                </button>


                                {showSuggestions && (

                                    <div className="absolute top-8 left-0 w-full bg-white shadow rounded-lg z-10">

                                        {suggestions.length > 0 ? (

                                            suggestions.map(s => (
                                                <div
                                                    key={s.id}
                                                    onClick={() => addItem(s.name, true)}
                                                    className="px-3 py-2 hover:bg-gray-100 cursor-pointer"
                                                >
                                                    {s.name}
                                                </div>
                                            ))

                                        ) : (

                                            <>
                                                <div
                                                    onClick={() => addItem(newItem, false)}
                                                    className="px-3 py-2 hover:bg-gray-100 cursor-pointer"
                                                >
                                                    Add "{newItem}"
                                                </div>

                                                <div
                                                    onClick={() => requestProduct(newItem)}
                                                    className="px-3 py-2 hover:bg-gray-100 cursor-pointer text-blue-500"
                                                >
                                                    Request this product
                                                </div>
                                            </>

                                        )}

                                    </div>

                                )}

                            </div>


                            {/* ACTIVE ITEMS */}

                            <div className="max-h-[420px] overflow-y-auto pr-2">

                                {sortedItems.filter(i => !i.isChecked).map(renderRow)}

                            </div>


                            {/* COMPLETED SECTION */}

                            {sortedItems.some(i => i.isChecked) && (

                                <>

                                    <button
                                        onClick={() => setCollapseCompleted(!collapseCompleted)}
                                        className="flex items-center gap-2 mt-6 text-sm"
                                    >
                                        Completed ({completedCount})
                                        <ChevronDown size={16} />
                                    </button>

                                    {!collapseCompleted && (

                                        <>
                                            {sortedItems.filter(i => i.isChecked).map(renderRow)}

                                            <button
                                                onClick={clearCompleted}
                                                className="text-xs text-blue-500 mt-2"
                                            >
                                                Clear completed items
                                            </button>
                                        </>

                                    )}

                                </>

                            )}

                        </motion.div>

                    </div>

                </div>

            </div>

        </StoreLayout>

    );


    function renderRow(item) {

        return (

            <div
                key={item.id}
                className="flex justify-between items-center group px-2 py-2 rounded-lg hover:bg-blue-50"
            >

                <div
                    onClick={() => toggleItem(item)}
                    className="flex items-center gap-4 cursor-pointer"
                >

                    <div
                        className={`w-6 h-6 flex items-center justify-center rounded-md border-2
${item.isChecked ? "border-blue-400 bg-blue-100" : "border-blue-200"}
`}
                    >
                        {item.isChecked && "✓"}
                    </div>

                    <span
                        className={`text-lg ${item.isChecked
                                ? "line-through text-gray-400"
                                : "text-gray-700"
                            }`}
                    >
                        {item.productName}
                    </span>

                </div>


                <div className="flex gap-2 items-center">

                    <select
                        value={item.priority}
                        onChange={e => changePriority(item.id, e.target.value)}
                        className="border rounded px-1 text-xs"
                    >
                        <option value="high">High</option>
                        <option value="medium">Medium</option>
                        <option value="low">Low</option>
                    </select>


                    <ShoppingCart
                        size={16}
                        onClick={() => addToCart(item)}
                        className="text-green-500 cursor-pointer"
                    />


                    <Trash2
                        size={16}
                        onClick={() => deleteItem(item.id)}
                        className="text-gray-300 opacity-0 group-hover:opacity-100 hover:text-red-400"
                    />

                </div>

            </div>

        );

    }

}