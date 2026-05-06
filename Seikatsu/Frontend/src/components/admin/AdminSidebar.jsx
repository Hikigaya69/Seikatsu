import {
  LayoutDashboard,
  Package,
  FolderKanban,
  Boxes,
  Repeat,
  BarChart3,
  Settings
} from "lucide-react";

import { NavLink } from "react-router-dom";

const links = [

  {
    name:"Dashboard",
    icon:LayoutDashboard,
    path:"/admin"
  },

  {
    name:"Products",
    icon:Package,
    path:"/admin/products"
  },

  {
    name:"Categories",
    icon:FolderKanban,
    path:"/admin/categories"
  },

  {
    name:"Inventory",
    icon:Boxes,
    path:"/admin/inventory"
  },

  {
    name:"Restock",
    icon:Repeat,
    path:"/admin/restock-management"
  },

  {
    name:"Analytics",
    icon:BarChart3,
    path:"/admin/analytics"
  },

  {
    name:"Settings",
    icon:Settings,
    path:"/admin/settings"
  }

];

export default function AdminSidebar() {

  return (

    <aside className="w-72 bg-[#111827] text-white p-6 flex flex-col">

      <div className="mb-12">

        <h1 className="text-3xl font-bold">
          Seikatsu
        </h1>

        <p className="text-gray-400 mt-2 text-sm">
          Admin Dashboard
        </p>

      </div>


      <div className="space-y-3">

        {links.map(link=>{

          const Icon = link.icon;

          return(

            <NavLink
              key={link.name}
              to={link.path}
              end
              className={({isActive})=>

                `flex items-center gap-3 px-4 py-3 rounded-xl transition ${
                  isActive
                  ? "bg-white text-black"
                  : "hover:bg-[#1f2937]"
                }`

              }
            >

              <Icon size={20}/>

              {link.name}

            </NavLink>

          );

        })}

      </div>

    </aside>

  );

}