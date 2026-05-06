import {
  Bell,
  Search,
  UserCircle2,
  LogOut
} from "lucide-react";

import { useNavigate } from "react-router-dom";

import api from "../../Utils/api";

export default function AdminNavbar() {

  const navigate = useNavigate();


  const handleLogout = async()=>{

    try{

      await api.post(
        "/Auth/logout",
        {},
        {
          withCredentials:true
        }
      );

      navigate("/login");

    }catch(err){

      console.error(err);

    }

  };


  return (

    <div className="h-20 bg-white border-b flex items-center justify-between px-8 shadow-sm">

      <div className="relative w-[420px]">

        <Search
          size={18}
          className="absolute left-4 top-3 text-gray-400"
        />

        <input
          type="text"
          placeholder="Search products, orders..."
          className="w-full pl-11 pr-4 py-3 rounded-xl border bg-[#f8fafc] outline-none"
        />

      </div>


      <div className="flex items-center gap-5">

        <Bell
          className="cursor-pointer"
        />


        <div className="flex items-center gap-3">

          <UserCircle2 size={34}/>

          <button
            onClick={handleLogout}
            className="flex items-center gap-2 bg-black text-white px-4 py-2 rounded-xl hover:opacity-90 transition"
          >

            <LogOut size={16}/>

            Logout

          </button>

        </div>

      </div>

    </div>

  );

}