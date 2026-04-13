import { useEffect, useState } from "react";
import StoreLayout from "../layouts/StoreLayout";
import api from "../Utils/api";
import { Trash2, Plus } from "lucide-react";
import { motion, AnimatePresence, Reorder } from "framer-motion";
import paperBg from "../assets/checklist_blue.png";

export default function Checklist() {

  const [items, setItems] = useState([]);
  const [newItem, setNewItem] = useState("");
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchChecklist();
  }, []);

  const fetchChecklist = async () => {

    try {
      const res = await api.get("/Checklist/getchecklist");
      setItems(res.data.data.items);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const addItem = async () => {

    if (!newItem.trim()) return;

    try {

      const res = await api.post("/Checklist/additem", {
        productName: newItem,
        isChecked: false
      });

      setItems(prev => [...prev, res.data.data]);
      setNewItem("");

    } catch (err) {
      console.error(err);
    }
  };

  const toggleItem = async (item) => {

    try {

      const res = await api.patch(
        `/Checklist/modifyitem?itemId=${item.id}`,
        {
          productName: item.productName,
          isChecked: !item.isChecked
        }
      );

      setItems(prev =>
        prev.map(i =>
          i.id === item.id ? res.data.data : i
        )
      );

    } catch (err) {
      console.error(err);
    }
  };

  const deleteItem = async (id) => {

    try {

      await api.delete(`/Checklist/deleteitem/${id}`);

      setItems(prev =>
        prev.filter(i => i.id !== id)
      );

    } catch (err) {
      console.error(err);
    }
  };

  return (

    <StoreLayout>

      {/* PAPER BACKGROUND */}

      <div
  className="min-h-screen flex justify-center items-start pt-24 relative"
  style={{
    backgroundImage: `url(${paperBg})`,
    backgroundSize: "cover",
    backgroundPosition: "center"
  }}
>

  <div className="absolute inset-0 backdrop-blur-[2px] bg-white/10" />

  

        

        <div className="relative">

          <div className="absolute top-3 left-3 w-[480px] h-[580px] bg-white/40 rounded-2xl rotate-2"></div>

          <div className="absolute top-1 left-1 w-[480px] h-[580px] bg-white/60 rounded-2xl -rotate-1"></div>


          {/* MAIN NOTE CARD */}

          <motion.div
            initial={{ opacity: 0, y: 40 }}
            animate={{ opacity: 1, y: 0 }}
            className="relative bg-white w-[480px] p-10 rounded-2xl shadow-2xl"
            style={{ fontFamily: "Patrick Hand" }}
          >

            

            <div className="absolute -top-6 right-8 text-3xl opacity-70">
              📎
            </div>


         

            <h1 className="text-3xl text-gray-700 mb-6">
              Shopping Checklist
            </h1>


            {/* INPUT */}

            <div className="flex gap-3 mb-6">

              <input
                value={newItem}
                onChange={(e) => setNewItem(e.target.value)}
                placeholder="Write something..."
                className="flex-1 border-b border-gray-300 bg-transparent focus:outline-none focus:border-blue-400 text-lg"
              />

              <button
                onClick={addItem}
                className="bg-blue-100 text-blue-600 px-3 py-1 rounded-lg hover:bg-blue-200 transition"
              >
                <Plus size={18} />
              </button>

            </div>


            {/* LIST */}

            {loading ? (

              <p className="text-gray-400">Loading...</p>

            ) : (

              <Reorder.Group
                axis="y"
                values={items}
                onReorder={setItems}
                className="space-y-4"
              >

                <AnimatePresence>

                  {items.map(item => (

                    <Reorder.Item
                      key={item.id}
                      value={item}
                      initial={{ opacity: 0, x: -20 }}
                      animate={{ opacity: 1, x: 0 }}
                      exit={{ opacity: 0 }}
                      className="flex justify-between items-center group"
                    >

                      {/* CHECK ROW */}

                      <div
                        onClick={() => toggleItem(item)}
                        className="flex items-center gap-3 cursor-pointer"
                      >

                        {/* CHECKBOX */}

                        <motion.div
                          layout
                          className={`w-5 h-5 rounded-full border transition-all
                          ${
                            item.isChecked
                              ? "bg-blue-400 border-blue-400"
                              : "border-gray-400"
                          }`}
                        />

                       

                        <motion.span
                          layout
                          className={`text-lg
                          ${
                            item.isChecked
                              ? "line-through text-gray-400"
                              : "text-gray-700"
                          }`}
                        >
                          {item.productName}
                        </motion.span>

                      </div>


                      {/* DELETE */}

                      <Trash2
                        size={16}
                        onClick={() => deleteItem(item.id)}
                        className="text-gray-300 opacity-0 group-hover:opacity-100 hover:text-red-400 transition cursor-pointer"
                      />

                    </Reorder.Item>

                  ))}

                </AnimatePresence>

              </Reorder.Group>

            )}

          </motion.div>

        </div>

      </div>

    </StoreLayout>

  );
}